export async function fetchApi(url, options = {}) {
    try {
        const response = await fetch(url, options);
        if (!response.ok) {
            const errorData = await response.json();
            throw new Error(errorData.message || 'API request failed');
        }
        return await response.json();
    } catch (error) {
        throw new Error(error.message || 'Network error');
    }
}
