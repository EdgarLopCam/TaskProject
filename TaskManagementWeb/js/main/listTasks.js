import { getTasks, updateTask, deleteTask } from '../controllers/taskController.js';
import { clearElement } from '../utils/domUtils.js';

let config = {};

async function loadConfig() {
    const configResponse = await fetch('js/config.json');
    config = await configResponse.json();
}

document.addEventListener('DOMContentLoaded', async function () {
    await loadConfig();
    const taskList = document.getElementById('taskList');
    const filterButton = document.getElementById('filterButton');
    const paginationDiv = document.getElementById('pagination');
    const emptyMessage = document.getElementById('emptyMessage');
    let currentPage = config.defaultPage;
    const pageSize = config.pageSize;

    async function fetchAndRenderTasks(pageNumber = 1, filters = {}) {
    try {
        const data = await getTasks(pageNumber, pageSize, filters);
        clearElement(taskList);
        if (data.items.length === 0) {
            emptyMessage.style.display = 'block';
        } else {
            emptyMessage.style.display = 'none';
            data.items.forEach(task => {
                const row = document.createElement('div');
                row.classList.add('task-grid-body');
                row.innerHTML = `
                    <div class="task-grid-cell"><strong>${task.title}</strong></div>
                    <div class="task-grid-cell">${task.description}</div>
                    <div class="task-grid-cell">${getPriorityName(task.priorityId)}</div>
                    <div class="task-grid-cell">${getStatusName(task.statusId)}</div>
                    <div class="task-grid-cell">${task.dueDate}</div>
                    <div class="task-grid-cell">
                        <button onclick="updateTask(${task.taskId}, '${task.rowVersion}')">Update</button>
                        <button onclick="deleteTask(${task.taskId}, '${task.rowVersion}')">Delete</button>
                    </div>
                `;
                taskList.appendChild(row);
            });
        }

        renderPagination(data.totalRecords, pageNumber, filters);
    } catch (error) {
        console.error('Error fetching tasks:', error);
        alert('Failed to fetch tasks.');
    }
}

    function getPriorityName(priorityId) {
        switch (priorityId) {
            case 1:
                return 'Low';
            case 2:
                return 'Medium';
            case 3:
                return 'High';
            default:
                return '';
        }
    }

    function getStatusName(statusId) {
        switch (statusId) {
            case 1:
                return 'Not Started';
            case 2:
                return 'In Progress';
            case 3:
                return 'Completed';
            default:
                return '';
        }
    }

    function renderPagination(totalPages, currentPage, filters) {
        clearElement(paginationDiv);
        for (let i = 1; i <= totalPages; i++) {
            const button = document.createElement('button');
            button.innerText = i;
            button.disabled = i === currentPage;
            button.addEventListener('click', () => fetchAndRenderTasks(i, filters));
            paginationDiv.appendChild(button);
        }
    }

    filterButton.addEventListener('click', function () {
        const priority = document.getElementById('filterPriority').value;
        const status = document.getElementById('filterStatus').value;

        const filters = {
            priority: priority ? parseInt(priority) : null,
            status: status ? parseInt(status) : null,
        };

        fetchAndRenderTasks(1, filters);
    });

    fetchAndRenderTasks();

    window.updateTask = async function (taskId, rowVersion) {
        const title = prompt('Enter new title:');
        const description = prompt('Enter new description:');
        const priority = prompt('Enter new priority, choose a number (1: Low, 2: Medium, 3: High):');
        const status = prompt('Enter new status, choose a number (1: Not Started, 2: In Progress, 3: Completed):');
        const dueDate = prompt('Enter new due date (YYYY-MM-DD):');

        // Validate inputs
        const errors = [];

        if (!title || title.trim() === '') {
            errors.push('Title is required.');
        }

        if (!description || description.trim() === '') {
            errors.push('Description is required.');
        }

        const priorityInt = parseInt(priority);
        if (isNaN(priorityInt) || priorityInt < 1 || priorityInt > 3) {
            errors.push('Priority must be a number between 1 and 3.');
        }

        const statusInt = parseInt(status);
        if (isNaN(statusInt) || statusInt < 1 || statusInt > 3) {
            errors.push('Status must be a number between 1 and 3.');
        }

        const dueDatePattern = /^\d{4}-\d{2}-\d{2}$/;
        if (!dueDatePattern.test(dueDate)) {
            errors.push('Due date must be in the format YYYY-MM-DD.');
        }

        if (errors.length > 0) {
            alert('Please correct the following errors:\n' + errors.join('\n'));
            return;
        }

        const task = {
            taskId,
            title,
            description,
            priorityId: priorityInt,
            statusId: statusInt,
            dueDate,
            rowVersion: rowVersion ? rowVersion : ""
        };

        try {
            await updateTask(taskId, task);
            alert('Task updated successfully!');
            location.reload();
        } catch (error) {
            console.error(`Error: ${error.message}`);
            location.reload();
        }
    };

    window.deleteTask = async function (taskId, rowVersion) {
        if (confirm('Are you sure you want to delete this task?')) {
            try {
                await deleteTask(taskId, { taskId, rowVersion });
                alert('Task deleted successfully!');
			     location.reload();
            } catch (error) {
                console.error(`Error: ${error.message}`);
				location.reload();
            }
        }
    };
});
