import { createTask } from '../controllers/taskController.js';
import { showMessage } from '../utils/domUtils.js';

let config = {};

async function loadConfig() {
    const configResponse = await fetch('js/config.json');
    config = await configResponse.json();
}

document.addEventListener('DOMContentLoaded', async function () {
    await loadConfig();
    const form = document.getElementById('create-task-form');
    const messageDiv = document.getElementById('message');

    form.addEventListener('submit', async function (e) {
        e.preventDefault();

        const title = document.getElementById('title').value;
        const description = document.getElementById('description').value;
        const priority = document.getElementById('priority').value;
        const status = document.getElementById('status').value;
        const dueDate = document.getElementById('dueDate').value;

        const task = {
            title,
            description,
            priorityId: parseInt(priority),
            statusId: parseInt(status),
            dueDate
        };

        try {
            await createTask(task);
            showMessage(messageDiv, 'Task created successfully!');
            form.reset();
        } catch (error) {
            showMessage(messageDiv, `Error: ${error.message}`);
        }
    });
});
