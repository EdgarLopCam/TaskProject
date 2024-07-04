export function clearElement(element) {
    while (element.firstChild) {
        element.removeChild(element.firstChild);
    }
}

export function showMessage(element, message) {
    element.innerHTML = message;
}
