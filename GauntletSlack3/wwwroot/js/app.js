window.handleBeforeUnload = async (dotNetHelper) => {
    try {
        const beforeUnloadEvent = (e) => {
            e.preventDefault();
            e.returnValue = '';
            dotNetHelper.invokeMethodAsync('HandleWindowUnload');
        };
        window.addEventListener('beforeunload', beforeUnloadEvent);
        return beforeUnloadEvent;
    } catch (error) {
        console.error('Error handling window unload:', error);
    }
}; 