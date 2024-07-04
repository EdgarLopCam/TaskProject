import { fetchApi } from '../services/apiService.js';

let apiUrl = '';

async function loadConfig() {
    const configResponse = await fetch('js/config.json');
    const config = await configResponse.json();
    apiUrl = config.apiUrl;
}

export async function getTasks(pageNumber = 1, pageSize = 10, filters = {}) {
    await loadConfig();
    let url = `${apiUrl}Task?pageNumber=${pageNumber}&pageSize=${pageSize}`;

    if (filters.priority) {
        url += `&priority=${filters.priority}`;
    }

    if (filters.status) {
        url += `&status=${filters.status}`;
    }

    if (filters.dueDate) {
        url += `&dueDate=${filters.dueDate}`;
    }

    return fetchApi(url);
}

export async function createTask(task) {
    await loadConfig();
    return fetchApi(`${apiUrl}Task`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(task)
    });
}

export async function updateTask(taskId, task) {
    await loadConfig();
    return fetchApi(`${apiUrl}Task/${taskId}`, {
        method: 'PUT',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(task)
    });
}

export async function deleteTask(taskId, rowVersion) {
    await loadConfig();

    return fetchApi(`${apiUrl}Task/${taskId}`, {
        method: 'DELETE',
		headers: {
            'Content-Type': 'application/json'
        },
		body: JSON.stringify(rowVersion)
    });

    return response;
}
