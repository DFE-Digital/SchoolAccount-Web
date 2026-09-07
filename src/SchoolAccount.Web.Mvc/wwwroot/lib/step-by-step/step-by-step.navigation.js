// Based on https://github.com/alphagov/govuk_publishing_components/blob/v29.11.0/app/assets/javascripts/govuk_publishing_components/components/step-by-step-nav.js
// Reduced to the parts this service uses: the cross-page link-list variant, the
// Google Analytics tracking and the session-storage 'remember open steps' feature
// have been removed. The fix for markup in step titles has been backported from
// v68.3.0; everything else that remains is unchanged from upstream.

window.GOVUK = window.GOVUK || {}
window.GOVUK.Modules = window.GOVUK.Modules || {};

(function (Modules) {
    function AppStepNav ($module) {
        this.$module = $module
        this.$module.actions = {} // stores text for JS appended elements 'show' and 'hide' on steps, and 'show/hide all' button
    }

    AppStepNav.prototype.init = function () {
        // Indicate that js has worked
        this.$module.classList.add('app-step-nav--active')

        // Prevent FOUC, remove class hiding content
        this.$module.classList.remove('js-hidden')

        this.$module.steps = this.$module.querySelectorAll('.js-step')
        this.$module.stepHeaders = this.$module.querySelectorAll('.js-toggle-panel')
        this.$module.totalSteps = this.$module.querySelectorAll('.js-panel').length
        this.$module.showOrHideAllButton = false

        this.getTextForInsertedElements()
        this.addButtonstoSteps()
        this.addShowHideAllButton()
        this.addShowHideToggle()
        this.addAriaControlsAttrForShowHideAllButton()

        this.showPreviouslyOpenedSteps()

        this.bindToggleForSteps()
        this.bindToggleShowHideAllButton()
    }

    AppStepNav.prototype.getTextForInsertedElements = function () {
        this.$module.actions.showText = this.$module.getAttribute('data-show-text')
        this.$module.actions.hideText = this.$module.getAttribute('data-hide-text')
        this.$module.actions.showAllText = this.$module.getAttribute('data-show-all-text')
        this.$module.actions.hideAllText = this.$module.getAttribute('data-hide-all-text')
    }

    AppStepNav.prototype.addShowHideAllButton = function () {
        var showAll = document.createElement('div')
        var steps = this.$module.querySelectorAll('.app-step-nav__steps')[0]

        showAll.className = 'app-step-nav__controls govuk-!-display-none-print'
        showAll.innerHTML =
            '<button aria-expanded="false" class="app-step-nav__button app-step-nav__button--controls js-step-controls-button">' +
            '<span class="app-step-nav__chevron app-step-nav__chevron--down js-step-controls-button-icon"></span>' +
            '<span class="app-step-nav__button-text app-step-nav__button-text--all js-step-controls-button-text">' +
            this.$module.actions.showAllText +
            '</span>' +
            '</button>'

        this.$module.insertBefore(showAll, steps)
        this.$module.showOrHideAllButton = this.$module.querySelectorAll('.js-step-controls-button')[0]
    }

    AppStepNav.prototype.addShowHideToggle = function () {
        for (var i = 0; i < this.$module.stepHeaders.length; i++) {
            var thisel = this.$module.stepHeaders[i]

            if (!thisel.querySelectorAll('.js-toggle-link').length) {
                var showHideSpan = document.createElement('span')
                var showHideSpanText = document.createElement('span')
                var showHideSpanIcon = document.createElement('span')
                var showHideSpanFocus = document.createElement('span')
                var thisSectionSpan = document.createElement('span')

                showHideSpan.className = 'app-step-nav__toggle-link js-toggle-link govuk-!-display-none-print'
                showHideSpanText.className = 'app-step-nav__button-text js-toggle-link-text'
                showHideSpanIcon.className = 'app-step-nav__chevron js-toggle-link-icon'
                showHideSpanFocus.className = 'app-step-nav__toggle-link-focus'
                thisSectionSpan.className = 'govuk-visually-hidden'

                showHideSpan.appendChild(showHideSpanFocus)
                showHideSpanFocus.appendChild(showHideSpanIcon)
                showHideSpanFocus.appendChild(showHideSpanText)

                thisSectionSpan.innerHTML = ' this section'
                showHideSpan.appendChild(thisSectionSpan)

                thisel.querySelectorAll('.js-step-title-button')[0].appendChild(showHideSpan)
            }
        }
    }

    AppStepNav.prototype.addAriaControlsAttrForShowHideAllButton = function () {
        var ariaControlsValue = this.$module.querySelectorAll('.js-panel')[0].getAttribute('id')

        this.$module.showOrHideAllButton.setAttribute('aria-controls', ariaControlsValue)
    }

    // called by show all/hide all, sets all steps accordingly
    AppStepNav.prototype.setAllStepsShownState = function (isShown) {
        for (var i = 0; i < this.$module.steps.length; i++) {
            var stepView = new this.StepView(this.$module.steps[i], this.$module)
            stepView.setIsShown(isShown)
        }
    }

    // called on load, determines whether each step should be open or closed
    AppStepNav.prototype.showPreviouslyOpenedSteps = function () {
        for (var i = 0; i < this.$module.steps.length; i++) {
            var thisel = this.$module.steps[i]
            var stepView = new this.StepView(thisel, this.$module)
            var shouldBeShown = thisel.hasAttribute('data-show')

            // show the step if it has the 'data-show' attribute
            stepView.setIsShown(shouldBeShown)
        }
    }

    AppStepNav.prototype.addButtonstoSteps = function () {
        for (var i = 0; i < this.$module.steps.length; i++) {
            var thisel = this.$module.steps[i]
            var title = thisel.querySelectorAll('.js-step-title')[0]
            var contentId = thisel.querySelectorAll('.js-panel')[0].getAttribute('id')
            var titleText = title.textContent

            title.outerHTML =
                '<span class="js-step-title">' +
                '<button ' +
                'class="app-step-nav__button app-step-nav__button--title js-step-title-button" ' +
                'aria-expanded="false" aria-controls="' + contentId + '">' +
                '<span class="app-step-nav____title-text-focus">' +
                '<span class="app-step-nav__title-text js-step-title-text"></span>' +
                '<span class="govuk-visually-hidden app-step-nav__section-heading-divider">, </span>' +
                '</span>' +
                '</button>' +
                '</span>'

            // Set the title as text, never as markup: it comes from the API.
            thisel.querySelector('.app-step-nav__title-text').textContent = titleText
        }
    }

    AppStepNav.prototype.bindToggleForSteps = function () {
        var that = this
        var togglePanels = this.$module.querySelectorAll('.js-toggle-panel')

        for (var i = 0; i < togglePanels.length; i++) {
            togglePanels[i].addEventListener('click', function () {
                var stepView = new that.StepView(this.parentNode, that.$module)
                stepView.toggle()

                that.setShowHideAllText()
            })
        }
    }

    AppStepNav.prototype.bindToggleShowHideAllButton = function () {
        var that = this

        this.$module.showOrHideAllButton.addEventListener('click', function () {
            var textContent = this.textContent || this.innerText
            var shouldShowAll = textContent === that.$module.actions.showAllText

            that.setAllStepsShownState(shouldShowAll)
            that.$module.showOrHideAllButton.setAttribute('aria-expanded', shouldShowAll)
            that.setShowHideAllText()

            return false
        })
    }

    AppStepNav.prototype.setShowHideAllText = function () {
        var shownSteps = this.$module.querySelectorAll('.step-is-shown').length
        var showAllChevon = this.$module.showOrHideAllButton.querySelector('.js-step-controls-button-icon')
        var showAllButtonText = this.$module.showOrHideAllButton.querySelector('.js-step-controls-button-text')
        // Find out if the number of is-opens == total number of steps
        var shownStepsIsTotalSteps = shownSteps === this.$module.totalSteps

        if (shownStepsIsTotalSteps) {
            showAllButtonText.innerHTML = this.$module.actions.hideAllText
            showAllChevon.classList.remove('app-step-nav__chevron--down')
        } else {
            showAllButtonText.innerHTML = this.$module.actions.showAllText
            showAllChevon.classList.add('app-step-nav__chevron--down')
        }
    }

    AppStepNav.prototype.StepView = function (stepElement, $module) {
        this.stepElement = stepElement
        this.stepContent = this.stepElement.querySelectorAll('.js-panel')[0]
        this.titleButton = this.stepElement.querySelectorAll('.js-step-title-button')[0]
        var textElement = this.stepElement.querySelectorAll('.js-step-title-text')[0]
        this.title = textElement.textContent || textElement.innerText
        this.title = this.title.replace(/^\s+|\s+$/g, '') // this is 'trim' but supporting IE8
        this.showText = $module.actions.showText
        this.hideText = $module.actions.hideText

        this.show = function () {
            this.setIsShown(true)
        }

        this.hide = function () {
            this.setIsShown(false)
        }

        this.toggle = function () {
            this.setIsShown(this.isHidden())
        }

        this.setIsShown = function (isShown) {
            var toggleLink = this.stepElement.querySelectorAll('.js-toggle-link')[0]
            var toggleLinkText = toggleLink.querySelector('.js-toggle-link-text')
            var stepChevron = toggleLink.querySelector('.js-toggle-link-icon')

            if (isShown) {
                this.stepElement.classList.add('step-is-shown')
                this.stepContent.classList.remove('js-hidden')
                toggleLinkText.innerHTML = this.hideText
                stepChevron.classList.remove('app-step-nav__chevron--down')
            } else {
                this.stepElement.classList.remove('step-is-shown')
                this.stepContent.classList.add('js-hidden')
                toggleLinkText.innerHTML = this.showText
                stepChevron.classList.add('app-step-nav__chevron--down')
            }
            this.titleButton.setAttribute('aria-expanded', isShown)
        }

        this.isShown = function () {
            return this.stepElement.classList.contains('step-is-shown')
        }

        this.isHidden = function () {
            return !this.isShown()
        }
    }

    Modules.AppStepNav = AppStepNav
})(window.GOVUK.Modules)