async function replaceFeedback(url, options = {}) {
    const feedback = document.querySelector("#page-feedback");

    if (!feedback) {
        return;
    }

    const response = await fetch(url, {
        ...options,
        headers: {
            "X-Requested-With": "XMLHttpRequest",
            ...(options.headers || {})
        }
    });

    if (!response.ok) {
        throw new Error("Feedback request failed.");
    }

    const html = await response.text();

    feedback.outerHTML = html;
}

document.addEventListener("click", async (event) => {
    const link = event.target.closest("#open, #cancel");
    if (!link) return;

    event.preventDefault();
    await replaceFeedback(link.href);
});

document.addEventListener("submit", async (event) => {
    const form = event.target.closest("#tell-us-form");
    if (!form) return;

    event.preventDefault();
    form.querySelector('button[type="submit"]').disabled = true;

    await replaceFeedback(form.action, {
        method: "POST",
        body: new FormData(form)
    });
});