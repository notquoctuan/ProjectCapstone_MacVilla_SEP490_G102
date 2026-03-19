// Cart JavaScript Functions

// Add product to cart
async function addToCart(productId) {
    const button = event.target;
    const originalText = button.innerHTML;
    
    // Show loading state
    button.disabled = true;
    button.innerHTML = '<i class="fas fa-spinner fa-spin me-1"></i>Đang thêm...';
    
    try {
        const response = await fetch('/api/cart/items', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': 'Bearer ' + getAuthToken()
            },
            body: JSON.stringify({
                productId: productId,
                quantity: 1
            })
        });
        
        if (response.ok) {
            const cart = await response.json();
            showToast('success', 'Đã thêm sản phẩm vào giỏ hàng!');
            updateCartBadge(cart.items.length);
        } else if (response.status === 401) {
            showToast('warning', 'Vui lòng đăng nhập để thêm vào giỏ hàng!');
            setTimeout(() => {
                window.location.href = '/Auth/Login';
            }, 1500);
        } else {
            const error = await response.json();
            showToast('error', error.message || 'Không thể thêm sản phẩm vào giỏ!');
        }
    } catch (error) {
        console.error('Error adding to cart:', error);
        showToast('error', 'Đã xảy ra lỗi. Vui lòng thử lại!');
    } finally {
        button.disabled = false;
        button.innerHTML = originalText;
    }
}

// Update cart item quantity
async function updateCartItem(cartItemId, newQuantity) {
    if (newQuantity < 1) return;
    
    try {
        const response = await fetch('/api/cart/items', {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': 'Bearer ' + getAuthToken()
            },
            body: JSON.stringify({
                cartItemId: cartItemId,
                quantity: newQuantity
            })
        });
        
        if (response.ok) {
            const cart = await response.json();
            updateCartDisplay(cart);
            showToast('success', 'Đã cập nhật số lượng!');
        } else if (response.status === 401) {
            showToast('warning', 'Vui lòng đăng nhập!');
            window.location.href = '/Auth/Login';
        } else {
            const error = await response.json();
            showToast('error', error.message || 'Không thể cập nhật số lượng!');
        }
    } catch (error) {
        console.error('Error updating cart item:', error);
        showToast('error', 'Đã xảy ra lỗi. Vui lòng thử lại!');
    }
}

// Delete cart item
async function deleteCartItem(cartItemId) {
    if (!confirm('Bạn có chắc muốn xóa sản phẩm này khỏi giỏ hàng?')) {
        return;
    }
    
    try {
        const response = await fetch(`/api/cart/items/${cartItemId}`, {
            method: 'DELETE',
            headers: {
                'Authorization': 'Bearer ' + getAuthToken()
            }
        });
        
        if (response.ok) {
            const cart = await response.json();
            updateCartDisplay(cart);
            updateCartBadge(cart.items.length);
            showToast('success', 'Đã xóa sản phẩm khỏi giỏ hàng!');
        } else if (response.status === 401) {
            showToast('warning', 'Vui lòng đăng nhập!');
            window.location.href = '/Auth/Login';
        } else {
            const error = await response.json();
            showToast('error', error.message || 'Không thể xóa sản phẩm!');
        }
    } catch (error) {
        console.error('Error deleting cart item:', error);
        showToast('error', 'Đã xảy ra lỗi. Vui lòng thử lại!');
    }
}

// Load cart data
async function loadCart() {
    const cartContainer = document.getElementById('cart-items-container');
    if (!cartContainer) return;
    
    cartContainer.innerHTML = '<div class="text-center py-5"><i class="fas fa-spinner fa-spin fa-3x"></i><p class="mt-2">Đang tải giỏ hàng...</p></div>';
    
    try {
        const response = await fetch('/api/cart', {
            headers: {
                'Authorization': 'Bearer ' + getAuthToken()
            }
        });
        
        if (response.ok) {
            const cart = await response.json();
            updateCartDisplay(cart);
            updateCartBadge(cart.items.length);
        } else if (response.status === 401) {
            cartContainer.innerHTML = `
                <div class="alert alert-warning">
                    <i class="fas fa-exclamation-triangle me-2"></i>
                    Vui lòng <a href="/Auth/Login">đăng nhập</a> để xem giỏ hàng của bạn.
                </div>`;
        } else {
            cartContainer.innerHTML = '<div class="alert alert-danger">Không thể tải giỏ hàng. Vui lòng thử lại sau.</div>';
        }
    } catch (error) {
        console.error('Error loading cart:', error);
        cartContainer.innerHTML = '<div class="alert alert-danger">Đã xảy ra lỗi. Vui lòng thử lại sau.</div>';
    }
}

// Update cart display with items
function updateCartDisplay(cart) {
    const cartContainer = document.getElementById('cart-items-container');
    const cartTotalEl = document.getElementById('cart-total');
    
    if (!cartContainer) return;
    
    if (!cart.items || cart.items.length === 0) {
        cartContainer.innerHTML = `
            <div class="text-center py-5">
                <i class="fas fa-shopping-cart fa-4x text-muted mb-3"></i>
                <h4>Giỏ hàng trống</h4>
                <p class="text-muted">Hãy thêm sản phẩm vào giỏ hàng của bạn!</p>
                <a href="/" class="btn btn-primary">Tiếp tục mua sắm</a>
            </div>`;
        if (cartTotalEl) cartTotalEl.textContent = '0 ₫';
        return;
    }
    
    let html = '';
    cart.items.forEach(item => {
        const imageUrl = item.imageUrl || '/img/no-image.png';
        html += `
            <div class="cart-item" id="cart-item-${item.cartItemId}">
                <div class="row align-items-center">
                    <div class="col-3 col-md-2">
                        <img src="${imageUrl}" alt="${item.productName}" class="img-fluid rounded" style="max-height: 80px; object-fit: cover;">
                    </div>
                    <div class="col-9 col-md-4">
                        <h6 class="mb-1">${item.productName}</h6>
                        <p class="text-muted mb-0 small">${item.unitPrice.toLocaleString('vi-VN')} ₫</p>
                    </div>
                    <div class="col-6 col-md-3 mt-2 mt-md-0">
                        <div class="input-group input-group-sm" style="max-width: 120px;">
                            <button class="btn btn-outline-secondary" onclick="updateQuantity(${item.cartItemId}, ${item.quantity - 1})">
                                <i class="fas fa-minus"></i>
                            </button>
                            <input type="number" class="form-control text-center" value="${item.quantity}" 
                                   min="1" onchange="updateQuantity(${item.cartItemId}, this.value)">
                            <button class="btn btn-outline-secondary" onclick="updateQuantity(${item.cartItemId}, ${item.quantity + 1})">
                                <i class="fas fa-plus"></i>
                            </button>
                        </div>
                    </div>
                    <div class="col-4 col-md-2 mt-2 mt-md-0 text-end">
                        <strong>${item.subTotal.toLocaleString('vi-VN')} ₫</strong>
                    </div>
                    <div class="col-2 col-md-1 mt-2 mt-md-0 text-end">
                        <button class="btn btn-sm btn-outline-danger" onclick="deleteCartItem(${item.cartItemId})">
                            <i class="fas fa-trash"></i>
                        </button>
                    </div>
                </div>
            </div>`;
    });
    
    cartContainer.innerHTML = html;
    
    if (cartTotalEl) {
        cartTotalEl.textContent = cart.totalPrice.toLocaleString('vi-VN') + ' ₫';
    }
}

// Helper function to update quantity
async function updateQuantity(cartItemId, quantity) {
    quantity = parseInt(quantity);
    if (quantity < 1) {
        await deleteCartItem(cartItemId);
        return;
    }
    await updateCartItem(cartItemId, quantity);
}

// Update cart badge count
function updateCartBadge(count) {
    const badges = document.querySelectorAll('.cart-badge');
    badges.forEach(badge => {
        badge.textContent = count || 0;
        badge.style.display = count > 0 ? 'inline-block' : 'none';
    });
}

// Get auth token from session
function getAuthToken() {
    return sessionStorage.getItem('JWToken') || '';
}

// Show toast notification
function showToast(type, message) {
    // Create toast element
    const toast = document.createElement('div');
    toast.className = `toast-notification toast-${type}`;
    toast.innerHTML = `
        <i class="fas fa-${type === 'success' ? 'check-circle' : type === 'warning' ? 'exclamation-triangle' : 'times-circle'}"></i>
        <span>${message}</span>
    `;
    
    // Add styles dynamically
    toast.style.cssText = `
        position: fixed;
        top: 20px;
        right: 20px;
        z-index: 9999;
        padding: 15px 20px;
        border-radius: 8px;
        color: white;
        font-weight: 500;
        box-shadow: 0 4px 12px rgba(0,0,0,0.15);
        animation: slideIn 0.3s ease;
        background: ${type === 'success' ? '#28a745' : type === 'warning' ? '#ffc107' : '#dc3545'};
        color: ${type === 'warning' ? '#333' : '#fff'};
    `;
    
    document.body.appendChild(toast);
    
    // Remove after 3 seconds
    setTimeout(() => {
        toast.style.animation = 'slideOut 0.3s ease';
        setTimeout(() => toast.remove(), 300);
    }, 3000);
}

// Add animation keyframes
const style = document.createElement('style');
style.textContent = `
    @keyframes slideIn {
        from { transform: translateX(100%); opacity: 0; }
        to { transform: translateX(0); opacity: 1; }
    }
    @keyframes slideOut {
        from { transform: translateX(0); opacity: 1; }
        to { transform: translateX(100%); opacity: 0; }
    }
`;
document.head.appendChild(style);

// Initialize cart on page load
document.addEventListener('DOMContentLoaded', function() {
    // Load cart badge count on homepage
    if (document.querySelector('.cart-badge')) {
        fetch('/api/cart', {
            headers: {
                'Authorization': 'Bearer ' + getAuthToken()
            }
        })
        .then(res => res.ok ? res.json() : null)
        .then(cart => {
            if (cart) {
                updateCartBadge(cart.items.length);
            }
        })
        .catch(() => {});
    }
});
