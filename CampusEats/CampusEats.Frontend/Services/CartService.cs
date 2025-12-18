using CampusEatsFrontend.Models.Cart;
using CampusEatsFrontend.Models.Menu;
using CampusEatsFrontend.Models.MenuItem;
using Microsoft.JSInterop;
using System.Text.Json;

namespace CampusEatsFrontend.Services;

public class CartService
{
    private readonly List<CartItem> _items = new();
    private readonly IJSRuntime _jsRuntime;
    private bool _initialized = false;
    
    public event Action? OnChange;

    public CartService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public IReadOnlyList<CartItem> Items => _items.AsReadOnly();
    
    public int TotalItems => _items.Sum(i => i.Quantity);
    
    public decimal TotalPrice => _items.Sum(i => i.Subtotal);

    public async Task InitializeAsync()
    {
        if (_initialized) return;

        try
        {
            var json = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", "cart");
            if (!string.IsNullOrEmpty(json))
            {
                var items = JsonSerializer.Deserialize<List<CartItem>>(json);
                if (items != null)
                {
                    _items.Clear();
                    _items.AddRange(items);
                }
            }
        }
        catch
        {
            // Ignore errors during initialization
        }
        
        _initialized = true;
        NotifyStateChanged();
    }

    public async Task AddMenuAsync(MenuDto menu, int quantity = 1)
    {
        if (!menu.Price.HasValue || menu.Price.Value <= 0)
            return;

        var existingItem = _items.FirstOrDefault(i => i.MenuId == menu.Id);
        
        if (existingItem != null)
        {
            existingItem.Quantity += quantity;
        }
        else
        {
            _items.Add(new CartItem
            {
                MenuId = menu.Id,
                MenuItemId = null,
                Name = menu.Name,
                Price = menu.Price.Value,
                Quantity = quantity,
                ImageUrl = menu.ImageUrl
            });
        }
        
        await SaveToLocalStorageAsync();
        NotifyStateChanged();
    }

    public async Task AddMenuItemAsync(MenuItemDto menuItem, int quantity = 1)
    {
        if (menuItem.Price <= 0)
            return;

        var existingItem = _items.FirstOrDefault(i => i.MenuItemId == menuItem.Id);
        
        if (existingItem != null)
        {
            existingItem.Quantity += quantity;
        }
        else
        {
            _items.Add(new CartItem
            {
                MenuId = null,
                MenuItemId = menuItem.Id,
                Name = menuItem.Name,
                Price = menuItem.Price,
                Quantity = quantity,
                ImageUrl = menuItem.ImageUrl
            });
        }
        
        await SaveToLocalStorageAsync();
        NotifyStateChanged();
    }

    // Keep old method for backward compatibility
    public async Task AddItemAsync(MenuDto menu, int quantity = 1)
    {
        await AddMenuAsync(menu, quantity);
    }

    public async Task RemoveItemAsync(Guid menuId)
    {
        _items.RemoveAll(i => i.MenuId == menuId);
        await SaveToLocalStorageAsync();
        NotifyStateChanged();
    }

    public async Task RemoveMenuItemAsync(Guid menuItemId)
    {
        _items.RemoveAll(i => i.MenuItemId == menuItemId);
        await SaveToLocalStorageAsync();
        NotifyStateChanged();
    }

    public async Task UpdateQuantityAsync(Guid id, int quantity, bool isMenuItem = false)
    {
        var item = isMenuItem 
            ? _items.FirstOrDefault(i => i.MenuItemId == id)
            : _items.FirstOrDefault(i => i.MenuId == id);
            
        if (item != null)
        {
            if (quantity <= 0)
            {
                if (isMenuItem)
                    await RemoveMenuItemAsync(id);
                else
                    await RemoveItemAsync(id);
            }
            else
            {
                item.Quantity = quantity;
                await SaveToLocalStorageAsync();
                NotifyStateChanged();
            }
        }
    }

    public async Task ClearAsync()
    {
        _items.Clear();
        await SaveToLocalStorageAsync();
        NotifyStateChanged();
    }

    private async Task SaveToLocalStorageAsync()
    {
        try
        {
            var json = JsonSerializer.Serialize(_items);
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "cart", json);
        }
        catch
        {
            // Ignore errors
        }
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}
