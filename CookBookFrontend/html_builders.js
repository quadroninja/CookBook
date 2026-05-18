
//<button onclick="editItem(${item.id})">Edit</button>
            //<button onclick="deleteItem(${item.id})">Delete</button>
export function createFoodItemCard(item) {
    return `
        <div class="food-card cursor-pointer hover:shadow-lg transition border rounded-lg overflow-hidden bg-white" data-id="${item.id}">
            <div class="flex">
                <!-- Image - fixed width, full height of card -->
                <div class="w-24 h-24 md:w-32 md:h-32 flex-shrink-0 bg-gray-100">
                    <img src="${item?.photoUrl || 'placeholder.jpg'}" 
                         class="w-full h-full object-cover"
                         onerror="this.onerror=null; this.src='placeholder.jpg'">
                </div>
                
                <!-- Content - takes remaining space -->
                <div class="flex-1 p-3">
                    <h3 class="font-semibold text-base md:text-lg line-clamp-1">${escapeHtml(item.name)}</h3>
                    <p class="text-xs md:text-sm text-gray-600 mt-1">
                        ${item.calories} kcal | P:${item.proteins} F:${item.fats} C:${item.carbohydrates}
                    </p>
                    <div class="flex gap-2 mt-2 w-1/2" onclick="event.stopPropagation()">
                        <button class="edit-btn flex-1 px-2 py-1 text-xs md:text-sm border rounded hover:bg-gray-50" data-id="${item.id}">Edit</button>
                        <button class="delete-btn flex-1 px-2 py-1 text-xs md:text-sm bg-red-500 text-white rounded hover:bg-red-600" data-id="${item.id}">Delete</button>
                    </div>
                </div>
            </div>
        </div>
    `;
}

export function createFilterItem(criterion, values)
{
    return `
        <div class="flex justify-between items-center mb-2 pb-2 border-b">
            <span class="text-sm font-medium text-gray-700 capitalize">${criterion}</span>
            <button class="remove-filter text-gray-400 hover:text-red-500 text-xl leading-none" 
                    onclick="removeFilter('${criterion}')">×</button>
        </div>
        <div class="filter-options space-y-1.5 max-h-64 overflow-y-auto">
            ${options.map(opt => `
                <label class="flex items-center gap-2 text-sm cursor-pointer hover:bg-gray-50 p-1 rounded">
                    <input type="checkbox" 
                           value="${opt}" 
                           class="filter-checkbox rounded"
                           data-criterion="${criterion}"
                           ${currentValues.includes(opt) ? 'checked' : ''}>
                    ${opt}
                </label>
            `).join('')}
        </div>
        <div class="text-xs text-gray-500 mt-2 pt-2 border-t">
            Selected: <span class="selected-count">${currentValues.length}</span>
        </div>
    `
}

export function escapeHtml(str) {
    if (!str) return '';
    return str.replace(/[&<>]/g, (m) => ({
        '&': '&amp;', '<': '&lt;', '>': '&gt;'
    })[m]);
}