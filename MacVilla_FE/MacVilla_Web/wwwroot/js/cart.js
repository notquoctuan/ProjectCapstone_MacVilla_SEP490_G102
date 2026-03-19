// ============================================================
// Cart JavaScript - Handles Add to Cart, Update Quantity, Delete
// ============================================================

const API_BASE_URL = window.API_BASE_URL || 'https://localhost:7262/';

// Toast notification function
function showToast(message, type = 'success') {
    // Remove existing toast if any
    const existingToast = document.querySelector('.cart-toast');
    if (existingToast) existingToast.remove();

    const toast = document.createElement('div');
    toast.className = `cart-toast toast-${type}`;
    toast.innerHTML = `
        <i class="fas fa-${type === 'success' ? 'check-circle' : type === 'error' ? 'exclamation-circle' : 'info-circle'}"></i>
        <span>${message}</span>
    `;
    
    // Add styles
    toast.style.cssText = `
        position: fixed;
        top: 20px;
        right: 20px;
        padding: 12px 20px;
        background: ${type === 'success' ? '#16a34a' : type === 'error' ? '#dc2626' : '#2563eb'};
        color: white;
        border-radius: 8px;
        box-shadow: 0 4px 12px rgba(0,0,0,0.15);
        z-index: 9999;
        display: flex;
        align-items: center;
        gap: 10px;
        animation: slideIn 0.3s ease;
        font-size: 14px;
        font-weight: 500;
    `;

    document.body.appendChild(toast);

    // Auto remove after 3 seconds
    setTimeout(() => {
        toast.style.animation = 'slideOut 0.3s ease';
        setTimeout(() => toast.remove(), 300);
    }, 3000);
}

// Get JWT token from cookie or session
function getToken() {
    // Try session first
    const token = sessionStorage.getItem('jwt') || localStorage.getItem('jwt');
    return token;
}

// Check if user is logged in
function isLoggedIn() {
    const token = getToken();
    return !!token;
}

// Get auth headers
function getAuthHeaders() {
    const token = getToken();
    if (!token) return {};
    return {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
    };
}

// Update cart badge
function updateCartBadge(count) {
    const badge = document.querySelector('.cart-badge');
    if (badge) {
        badge.textContent = count;
        badge.style.animation = 'none';
        badge.offsetHeight; // reflow
        badge.style.transition = 'transform 0.2s';
        badge.style.transform = 'scale(1.4)';
        setTimeout(() => badge.style.transform = 'scale(1)', 200);
    }
}

// ============================================================
// Add to Cart Function
// ============================================================
async function addToCart(productId, quantity = 1) {
    if (!isLoggedIn()) {
        showToast('Vui lòng đăng nhập để thêm sản phẩm vào giỏ hàng!', 'error');
        setTimeout(() => {
            window.location.href = '/Auth/Login';
        }, 1500);
        return;
    }

    try {
        const response = await fetch(`${API_BASE_URL}api/cart/items`, {
            method: 'POST',
            headers: getAuthHeaders(),
            body: JSON.stringify({ productId, quantity })
        });

        if (response.ok) {
            const cart = await response.json();
            const totalItems = cart.items.reduce((sum, item) => sum + item.quantity, 0);
            updateCartBadge(totalItems);
            showToast('Đã thêm sản phẩm vào giỏ hàng!', 'success');
        } else if (response.status === 401) {
            showToast('Phiên đăng nhập hết hạn. Vui lòng đăng nhập lại!', 'error');
            setTimeout(() => {
                window.location.href = '/Auth/Login';
            }, 1500);
        } else {
            const error = await response.json();
            showToast(error.message || 'Không thể thêm sản phẩm vào giỏ hàng!', 'error');
        }
    } catch (error) {
        console.error('Add to cart error:', error);
        showToast('Đã xảy ra lỗi khi thêm sản phẩm!', 'error');
    }
}

// ============================================================
// Load Cart Data (for Cart Page)
// ============================================================
async function loadCart() {
    if (!isLoggedIn()) {
        showCartEmpty('Vui lòng đăng nhập để xem giỏ hàng!');
        return;
    }

    const cartContainer = document.getElementById('cart-items-container');
    const cartSummary = document.getElementById('cart-summary');
    const loadingEl = document.getElementById('cart-loading');
    
    if (loadingEl) loadingEl.style.display = 'block';
    if (cartContainer) cartContainer.innerHTML = '';

    try {
        const response = await fetch(`${API_BASE_URL}api/cart`, {
            method: 'GET',
            headers: getAuthHeaders()
        });

        if (response.ok) {
            const cart = await response.json();
            renderCart(cart);
        } else if (response.status === 401) {
            showCartEmpty('Phiên đăng nhập hết hạn. Vui lòng đăng nhập lại!');
        } else {
            showCartEmpty('Không thể tải giỏ hàng!');
        }
    } catch (error) {
        console.error('Load cart error:', error);
        showCartEmpty('Đã xảy ra lỗi khi tải giỏ hàng!');
    } finally {
        if (loadingEl) loadingEl.style.display = 'none';
    }
}

// Render cart items
function renderCart(cart) {
    const cartContainer = document.getElementById('cart-items-container');
    const cartSummary = document.getElementById('cart-summary');
    
    if (!cartContainer) return;

    if (!cart.items || cart.items.length === 0) {
        showCartEmpty('Giỏ hàng của bạn đang trống!');
        if (cartSummary) cartSummary.style.display = 'none';
        return;
    }

    if (cartSummary) cartSummary.style.display = 'block';

    let html = '';
    cart.items.forEach(item => {
        html += `
            <div class="cart-item" data-cart-item-id="${item.cartItemId}">
                <div class="cart-item-image">
                    ${item.imageUrl 
                        ? `<img src="${item.imageUrl}" alt="${item.productName}" />` 
                        : `<div class="no-image"><i class="fas fa-box"></i></div>`
                    }
                </div>
                <div class="cart-item-details">
                    <div class="cart-item-name">${item.productName || 'Sản phẩm'}</div>
                    <div class="cart-item-price">${formatPrice(item.unitPrice)}</div>
                </div>
                <div class="cart-item-quantity">
                    <button class="qty-btn qty-minus" onclick="updateCartItem(${item.cartItemId}, ${item.quantity - 1})">
                        <i class="fas fa-minus"></i>
                    </button>
                    <input type="number" class="qty-input" value="${item.quantity}" 
                           min="1" max="99" 
                           onchange="updateCartItem(${item.cartItemId}, this.value)" />
                    <button class="qty-btn qty-plus" onclick="updateCartItem(${item.cartItemId}, ${item.quantity + 1})">
                        <i class="fas fa-plus"></i>
                    </button>
                </div>
                <div class="cart-item-subtotal">${formatPrice(item.subTotal)}</div>
                <div class="cart-item-actions">
                    <button class="btn-delete" onclick="deleteCartItem(${item.cartItemId})" title="Xóa sản phẩm">
                        <i class="fas fa-trash"></i>
                    </button>
                </div>
            </div>
        `;
    });

    cartContainer.innerHTML = html;

    // Update total
    const totalEl = document.getElementById('cart-total-price');
    if (totalEl) {
        totalEl.textContent = formatPrice(cart.totalPrice);
    }

    // Update summary
    const itemCountEl = document.getElementById('cart-item-count');
    if (itemCountEl) {
        const totalItems = cart.items.reduce((sum, item) => sum + item.quantity, 0);
        itemCountEl.textContent = totalItems;
    }

    const subtotalEl = document.getElementById('cart-subtotal');
    if (subtotalEl) {
        subtotalEl.textContent = formatPrice(cart.totalPrice);
    }

    // Update badge on all pages
    updateCartBadge(totalItems);
}

// Show empty cart message
function showCartEmpty(message) {
    const cartContainer = document.getElementById('cart-items-container');
    if (!cartContainer) return;

    cartContainer.innerHTML = `
        <div class="cart-empty">
            <i class="fas fa-shopping-cart"></i>
            <h3>${message}</h3>
            <p>Hãy tiếp tục mua sắm để thêm sản phẩm vào giỏ hàng của bạn!</p>
            <a href="/" class="btn-continue-shopping">
                <i class="fas fa-arrow-left"></i> Tiếp tục mua sắm
            </a>
        </div>
    `;
}

// ============================================================
// Update Cart Item Quantity
// ============================================================
async function updateCartItem(cartItemId, quantity) {
    quantity = parseInt(quantity);
    if (isNaN(quantity) || quantity < 1) quantity = 1;
    if (quantity > 99) quantity = 99;

    const itemEl = document.querySelector(`[data-cart-item-id="${cartItemId}"]`);
    if (itemEl) {
        const input = itemEl.querySelector('.qty-input');
        if (input) input.value = quantity;
        itemEl.classList.add('updating');
    }

    try {
        const response = await fetch(`${API_BASE_URL}api/cart/items`, {
            method: 'PUT',
            headers: getAuthHeaders(),
            body: JSON.stringify({ cartItemId, quantity })
        });

        if (response.ok) {
            const cart = await response.json();
            renderCart(cart);
            showToast('Đã cập nhật số lượng!', 'success');
        } else if (response.status === 401) {
            showToast('Phiên đăng nhập hết hạn!', 'error');
            setTimeout(() => window.location.href = '/Auth/Login', 1500);
        } else {
            const error = await response.json();
            showToast(error.message || 'Không thể cập nhật số lượng!', 'error');
            if (itemEl) itemEl.classList.remove('updating');
        }
    } catch (error) {
        console.error('Update cart error:', error);
        showToast('Đã xảy ra lỗi!', 'error');
        if (itemEl) itemEl.classList.remove('updating');
    }
}

// ============================================================
// Delete Cart Item
// ============================================================
async function deleteCartItem(cartItemId) {
    if (!confirm('Bạn có chắc chắn muốn xóa sản phẩm này khỏi giỏ hàng?')) {
        return;
    }

    const itemEl = document.querySelector(`[data-cart-item-id="${cartItemId}"]`);
    if (itemEl) itemEl.classList.add('deleting');

    try {
        const response = await fetch(`${API_BASE_URL}api/cart/items/${cartItemId}`, {
            method: 'DELETE',
            headers: getAuthHeaders()
        });

        if (response.ok) {
            const cart = await response.json();
            renderCart(cart);
            showToast('Đã xóa sản phẩm khỏi giỏ hàng!', 'success');
        } else if (response.status === 401) {
            showToast('Phiên đăng nhập hết hạn!', 'error');
            setTimeout(() => window.location.href = '/Auth/Login', 1500);
        } else {
            const error = await response.json();
            showToast(error.message || 'Không thể xóa sản phẩm!', 'error');
            if (itemEl) itemEl.classList.remove('deleting');
        }
    } catch (error) {
        console.error('Delete cart error:', error);
        showToast('Đã xảy ra lỗi!', 'error');
        if (itemEl) itemEl.classList.remove('deleting');
    }
}

// ============================================================
// Clear Entire Cart
// ============================================================
async function clearCart() {
    if (!confirm('Bạn có chắc chắn muốn xóa toàn bộ giỏ hàng?')) {
        return;
    }

    try {
        const response = await fetch(`${API_BASE_URL}api/cart`, {
            method: 'DELETE',
            headers: getAuthHeaders()
        });

        if (response.ok) {
            renderCart({ items: [], totalPrice: 0 });
            updateCartBadge(0);
            showToast('Đã xóa toàn bộ giỏ hàng!', 'success');
        } else {
            showToast('Không thể xóa giỏ hàng!', 'error');
        }
    } catch (error) {
        console.error('Clear cart error:', error);
        showToast('Đã xảy ra lỗi!', 'error');
    }
}

// ============================================================
// Utility Functions
// ============================================================
function formatPrice(price) {
    if (price === null || price === undefined) return '0 ₫';
    return new Intl.NumberFormat('vi-VN', { 
        style: 'currency', 
        currency: 'VND' 
    }).format(price);
}

// Initialize cart badge on page load
document.addEventListener('DOMContentLoaded', function() {
    // Add animation keyframes if not already added
    if (!document.getElementById('cart-animation-styles')) {
        const style = document.createElement('style');
        style.id = 'cart-animation-styles';
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
    }
    
    // If on cart page, load cart data
    if (document.getElementById('cart-items-container')) {
        loadCart();
    }
});
