// Click outside handler for notification dropdown
window.registerClickOutside = (elementRef, dotNetHelper) => {
    const handler = (event) => {
        if (!elementRef.contains(event.target)) {
            dotNetHelper.invokeMethodAsync('CloseNotificationDropdown');
        }
    };
    
    // Remove existing handler to avoid duplicates
    if (window._clickOutsideHandler) {
        document.removeEventListener('click', window._clickOutsideHandler);
    }
    
    window._clickOutsideHandler = handler;
    document.addEventListener('click', handler);
};

// Cleanup function
window.unregisterClickOutside = () => {
    if (window._clickOutsideHandler) {
        document.removeEventListener('click', window._clickOutsideHandler);
        window._clickOutsideHandler = null;
    }
};
