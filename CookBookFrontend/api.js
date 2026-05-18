export const API_BASE = 'https://localhost:7052';

export async function requestDeleteFoodItem(id)
{
    let url = `${API_BASE}/food_items/delete/${id}`;
    console.log(`Delete: fetching ${url}`)
    
    const response = await fetch(url, {method: 'DELETE'});
    
    return {
        success: response.ok ? true : false,
        status: response.status,
        body: await response.text()
    }
}

export async function fetchFoodItems(sortField, sortOrder, searchString, filters) {
    try {
        let url = `${API_BASE}/food_items/get?`;

        if (sortField) {
            url += `sortBy=${sortField}&desc=${sortOrder === 'desc'}&`;
        }
        
        // Add search if specified
        if (searchString) {
            url += `searchBy=${encodeURIComponent(searchString)}&`;
        }

        for (const [criterion, filterData] of Object.entries(filters)) {
            const values = filterData.values;
            if (values.length === 0) continue;
            
            values.forEach((value) => {
                switch (criterion) {
                    case 'category':
                        url += `category=${encodeURIComponent(value)}&`;
                        break;
                    case 'readinessToEat':
                        url += `ready=${encodeURIComponent(value)}&`; 
                        break;
                    case 'dietaryFlags':
                        url += `flags=${values.join(',')}&`; 
                        break;
                    }
                });            
        }
        console.log('1. Fetching from:', `${url}`);
        
        const response = await fetch(url);

        console.log('2. Response status:', response.status);
        
        if (!response.ok) throw new Error(`HTTP ${response.status}`);
        
        const data = await response.json();
        console.log('3. Received items:', data.length);
        
        return data;
    } catch (error) {
        console.error('Get food items :: API Error:', error);
        return []; // Return empty array on error
    }
}
