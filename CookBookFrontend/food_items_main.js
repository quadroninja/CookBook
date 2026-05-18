'use strict';
import { createFoodItemCard, escapeHtml } from "./html_builders.js";
import { fetchFoodItems, requestDeleteFoodItem } from "./api.js" 

const filterStates = {
  foodItems: {
    readinessToEat: { type: 'enum', values: ['NOT_READY', 'HALF_READY', 'READY'] },      // single-select?
    category: { type: 'enum', values: ['FROZEN', 'MEAT', 'VEGETABLE', 'GREENS', 'SPICE', 'CEREAL', 'CANNED', 'LIQUID', 'SWEETS'] },    
    dietaryFlags: { type: 'flag_enum', values: ['VEGAN', 'GLUTEN_FREE', 'SUGAR_FREE'] },  
  },
  dishes: {
    category: { type: 'enum', values: ['DESSERT', 'FIRST_COURSE', 'SECOND_COURSE', 'DRINK', 'SALAD', 'SOUP', 'SNACK'] },    
    dietaryFlags: { type: 'flag_enum', values: ['VEGAN', 'GLUTEN_FREE', 'SUGAR_FREE'] }       
  }
}

let currentSort = { field: 'name', order: 'asc' };
let currentSearchString = null;

let activeFilters = {}; 
let nextFilterId = 1;

let foodItems = [];

let currentEditId = null;


document.getElementById('category').innerHTML = filterStates.foodItems.category.values.map((value) => 
    `<option value="${value}">${value}</option>`)
document.getElementById('readinessToEat').innerHTML = filterStates.foodItems.readinessToEat.values.map((value) => 
    `<option value="${value}">${value}</option>`)
document.getElementById('dietaryFlagsContainer').innerHTML = filterStates.foodItems.dietaryFlags.values.map((value) => 
`
    <label class="flex items-center gap-2">
        <input type="checkbox" value="${value}" class="dietary-flag"> ${value}
    </label>`).join('');

document.getElementById("addFoodItemBtn").addEventListener("click", (e) => {
    addFoodItem();
});
document.querySelectorAll(".closeCreateModal").forEach((button) => button.addEventListener("click", (e) => {
    closeCreateModal();
}));
document.querySelectorAll(".closeDetailsModal").forEach((button) => button.addEventListener("click", (e) => {
    closeDetailsModal();
}));



document.getElementById('foodItemCreateForm').addEventListener('submit', async (e) => {
    e.preventDefault();
    let isEditing = currentEditId != null;

    const formData = new FormData();
    
    formData.append('Name', document.getElementById('name').value);
    formData.append('Calories', document.getElementById('calories').value);
    formData.append('Proteins', document.getElementById('proteins').value);
    formData.append('Fats', document.getElementById('fats').value);
    formData.append('Carbohydrates', document.getElementById('carbohydrates').value);
    formData.append('Category', document.getElementById('category').value);
    formData.append('ReadinessToEat', document.getElementById('readinessToEat').value);
    formData.append('Contents', document.getElementById('contents').value);
    
    
    const flags = Array.from(document.querySelectorAll('.dietary-flag:checked'))
        .map(cb => cb.value)
        .join(', ');

    formData.append('DietaryFlags', flags);
    const photos = [...document.getElementById('photos').files];
    if (photos.length > 0)
        photos.forEach((photo) => formData.append("Photos", photo));


    const url = `https://localhost:7052/food_items/${isEditing ? `edit/${currentEditId}` : 'create'}`;
    
    const method = isEditing ? "PATCH" : 'POST';
    
    try {
        console.log(`Fetching: ${url}`);
        const response = await fetch(url, { method: method, body: formData });
        

        if (response.ok) {
            closeCreateModal();
            await loadAndDisplay();
            alert(`Item ${isEditing ? "edited" : "created"}!`);
        } else {
            const error = await response.text();
            alert(`Failed: ${error}`);
        }
    } catch (error) {
        console.log(`Creation error: ${error}`);
        alert('Network error: ' + error.message);
    }
});







document.getElementById('sortSelect').addEventListener('change', (e) => {
    currentSort.field = e.target.value;
    console.log(`current sort field: ${currentSort.field}, target value: ${e.target.value}`);
    loadAndDisplay();  
});

document.getElementById('sortOrderBtn').addEventListener('click', () => {    
    currentSort.order = currentSort.order === 'asc' ? 'desc' : 'asc';
    
    const buttonText = currentSort.order === 'asc' ? '↓ Ascending' : '↑ Descending';
    document.getElementById('sortOrderBtn').textContent = buttonText;
    
    loadAndDisplay();  
});

document.getElementById('searchBtn').addEventListener('click', (e) => {
    currentSearchString = document.getElementById('searchBar').value;
    
    loadAndDisplay();
})

document.getElementById('addFilterBtn').addEventListener('click', (e) => {
    addFilter();
    loadAndDisplay();
})


//document.getElementById('searchBar').addEventListener()


async function showDetails(id) {
    try {
        console.log("showDetails invoked");
        const response = await fetch(`https://localhost:7052/food_items/get/${id}`);
        const item = await response.json();
        
        document.getElementById('detailTitle').innerText = item.name;
        
        document.getElementById('detailContent').innerHTML = `
            ${item.photoUrls.map((url) => `
                <div class="flex justify-center">
                    <img src="${url}" 
                         class="w-full object-contain max-h-64 rounded-lg bg-gray-100"
                         onerror="this.src='placeholder.jpg'">
                </div>
            `)}
            
            <div class="space-y-2">
                <div class="grid grid-cols-2 gap-2 text-sm">
                    <div class="text-gray-500">Calories (100g):</div>
                    <div class="font-medium">${item.calories} kcal</div>
                    
                    <div class="text-gray-500">Proteins (100g):</div>
                    <div class="font-medium">${item.proteins} g</div>
                    
                    <div class="text-gray-500">Fats (100g):</div>
                    <div class="font-medium">${item.fats} g</div>
                    
                    <div class="text-gray-500">Carbohydrates (100g):</div>
                    <div class="font-medium">${item.carbohydrates} g</div>
                </div>
                
                <div class="border-t pt-2">
                    <div class="text-gray-500 text-sm">Category:</div>
                    <div class="font-medium">${item.category || 'Not specified'}</div>
                </div>
                
                <div class="border-t pt-2">
                    <div class="text-gray-500 text-sm">Readiness:</div>
                    <div class="font-medium">${item.readinessToEat || 'Not specified'}</div>
                </div>
                
                <div class="border-t pt-2">
                    <div class="text-gray-500 text-sm">Contents/Ingredients:</div>
                    <div class="text-sm">${item.contents || 'Not specified'}</div>
                </div>
                
                ${item.dietaryFlags && item.dietaryFlags.length ? `
                    <div class="border-t pt-2">
                        <div class="text-gray-500 text-sm">Dietary Flags:</div>
                        <div class="flex flex-wrap gap-1 mt-1">
                            ${item.dietaryFlags.split(', ').map(flag => `
                                <span class="px-2 py-1 bg-green-100 text-green-800 text-xs rounded-full">${flag}</span>
                            `).join('')}
                        </div>
                    </div>
                ` : ''}
            </div>
        `;

        openDetailsModal();
    } catch (error) {
        console.error('Failed to load details:', error);
        alert('Failed to load product details');
    }
}


async function editFoodItem(id) {
    currentEditId = id;  // Store it
    
    const response = await fetch(`https://localhost:7052/food_items/get/${id}`);
    const item = await response.json();
    
    document.getElementById('modalTitle').innerText = 'Edit Food Item';
    document.getElementById('name').value = item.name;
    document.getElementById('calories').value = item.calories;
    document.getElementById('proteins').value = item.proteins;
    document.getElementById('fats').value = item.fats;
    document.getElementById('carbohydrates').value = item.carbohydrates;
    document.getElementById('category').value = item.category;
    document.getElementById('readinessToEat').value = item.readinessToEat;
    document.getElementById('contents').value = item.contents;
    
    const checkedFlags = item.dietaryFlags.split(', ');
    [...document.querySelectorAll('.dietary-flag')].map(cb => {cb.checked = checkedFlags.includes(cb.value)});
    
    

    openCreateModal();
}

// When Add button is clicked
function addFoodItem() {
    currentEditId = null; 
    document.getElementById('modalTitle').innerText = 'Add Food Item';
    document.getElementById('foodItemCreateForm').reset();
    openCreateModal();
}



function addFilter() {
    //Array.of(Object.keys(filterStates.foodItems))
    const usedCriteria = Object.keys(activeFilters);
    const available = Object.keys(filterStates.foodItems).filter(c => !usedCriteria.includes(c));
    console.log(`add filter:: usedCriteria:${usedCriteria}, available:${typeof(available)}`);
    if (available.length === 0) {
        alert('All filter criteria are already in use');
        return;
    }
    
    const newCriterion = available[0];
    
    activeFilters[newCriterion] = { values: [] };
    console.log(`new activeFilters: ${Object.entries(activeFilters)}`)
}


function updateFilterValues(criterion, selectedValues) {
    if (activeFilters[criterion]) {
        activeFilters[criterion].values = selectedValues;
    }
    loadAndDisplay();
}

function removeFilter(criterion) {
    delete activeFilters[criterion];
    
    const filterElement = document.getElementById(`filter-${criterion}`);
    if (filterElement) filterElement.remove();

    console.log(`current filters: ${JSON.stringify(activeFilters)}`)
}


async function deleteFoodItem(id) {
    const result = await requestDeleteFoodItem(id);
    if (!result.success)
    {
        switch (result.status)
        {
            case 404: //удаляемая сущность не найдена (скорее всего не произойдет)
                alert(`Food item id:${id} not found!`);
            break;
            case 400: //произошло нарушение ограничений целостности
                alert(`Integrity constraint violated: ${result.body}`);
            break;
            default:
                alert(`Failed to delete food item id:${id}`)
        }
        return;
    }

    console.log(`deleted food item: ${id}`);

    loadAndDisplay();
}

function openDetailsModal()
{
    const modal = document.getElementById("detailModal");
    modal.classList.remove('hidden');
    modal.classList.add('flex');
}
function closeDetailsModal()
{
    const modal = document.getElementById("detailModal");
    modal.classList.add('hidden');
    modal.classList.remove('flex');
}

function openCreateModal()
{
    const modal = document.getElementById("foodItemCreateModal");
    modal.classList.remove('hidden');
    modal.classList.add('flex');
}

function closeCreateModal()
{
    const modal = document.getElementById('foodItemCreateModal');
    modal.classList.add('hidden');
    modal.classList.remove('flex');
    document.getElementById('foodItemCreateForm').reset();
    document.getElementById('editId').value = '';
    document.getElementById('modalTitle').innerText = 'Add Food Item';
}


function renderFoodItems() {
    const container = document.getElementById('foodItemsContainer');
    
    if (!foodItems.length) {
        container.innerHTML = '<p class="text-center text-gray-500">No items found</p>';
        return;
    }
    
    container.innerHTML = foodItems.map(createFoodItemCard).join('');
    container.querySelectorAll(".delete-btn").forEach(btn => {
        btn.addEventListener('click', (e) => {
            e.stopPropagation();
            deleteFoodItem(btn.dataset.id);
        });
    }); 
    container.querySelectorAll(".edit-btn").forEach(btn => {
        btn.addEventListener('click', (e) => {
            e.stopPropagation();
            editFoodItem(btn.dataset.id);
        });
    }); 
    
    [...document.querySelectorAll(".food-card")].map((card) => 
        card.addEventListener('click', (e) => showDetails(e.currentTarget.dataset.id)));
}



function renderFilterItems() {
    document.getElementById('filterContainer').innerHTML = '';

    for (let [criterion, values] of Object.entries(activeFilters).map((entry) => [entry[0], entry[1].values]))
    {
        const filterDiv = document.createElement('div');
        filterDiv.className = 'filter-chip w-72 bg-white border border-gray-300 rounded-lg p-3';
        filterDiv.id = `filter-${criterion}`;
        
        const options = filterStates.foodItems[criterion].values;
        console.log(options);
        const currentValues = values;
        
        filterDiv.innerHTML = `
        <div class="flex justify-between items-center mb-2 pb-2 border-b">
            <span class="text-sm font-medium text-gray-700 capitalize">${criterion}</span>
            <button class="remove-filter text-gray-400 hover:text-red-500 text-xl leading-none" >×</button>
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
        `;
            
            
            const checkboxes = filterDiv.querySelectorAll('.filter-checkbox');
            checkboxes.forEach(cb => {
                cb.addEventListener('change', () => {
                    const criterion = cb.dataset.criterion;
                    const container = document.getElementById(`filter-${criterion}`);
                    const checkedBoxes = container.querySelectorAll('.filter-checkbox:checked');
                    const selectedValues = Array.from(checkedBoxes).map(c => c.value);
                    
                    updateFilterValues(criterion, selectedValues);
                    
                    const countSpan = container.querySelector('.selected-count');
                    if (countSpan) countSpan.textContent = selectedValues.length;
                });
            });
            
            const addBtn = document.getElementById('addFilterBtn');
            const filtersArea = document.getElementById('filterContainer');
            filtersArea.append(filterDiv);  
            filterDiv.querySelector('.remove-filter').addEventListener('click', () => removeFilter(criterion));
            console.log(`remove-filter button: ${filterDiv.querySelector('.remove-filter')}`);
        }
    }


let requestCount = 0;

async function loadAndDisplay() {
    console.log('🟢 Starting load...');
    
    foodItems = await fetchFoodItems(currentSort.field, currentSort.order, currentSearchString, activeFilters);
    
    renderFoodItems();
    renderFilterItems();
    
    console.log('✅ Loaded and rendered');
}

// Start when page loads
loadAndDisplay();
