function initScrollAnimations() {
    const observer = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                entry.target.classList.add('visible');
                observer.unobserve(entry.target);
            }
        });
    }, { threshold: 0.15 });

    document.querySelectorAll('.scroll-animate').forEach(el => observer.observe(el));
}

window.initScrollAnimations = initScrollAnimations;

function initFileUpload(dotNetRef) {
    const input = document.getElementById('file-upload-input');
    if (!input) return;
    input.addEventListener('dragenter', () => dotNetRef.invokeMethodAsync('SetDragOver', true));
    input.addEventListener('dragleave', () => dotNetRef.invokeMethodAsync('SetDragOver', false));
    input.addEventListener('drop', () => dotNetRef.invokeMethodAsync('SetDragOver', false));
}

window.initFileUpload = initFileUpload;
