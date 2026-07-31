const products = [
  { id: 1, name: 'Apple', price: 1.25, unit: 'each', emoji: '🍎', description: 'Crisp and sweet, perfect for snacking.' },
  { id: 2, name: 'Banana', price: 0.75, unit: 'each', emoji: '🍌', description: 'A soft, sweet fruit ideal for breakfast.' },
  { id: 3, name: 'Orange', price: 1.10, unit: 'each', emoji: '🍊', description: 'Juicy and bright with a zesty flavor.' },
  { id: 4, name: 'Strawberry', price: 2.50, unit: 'box', emoji: '🍓', description: 'Fresh and fragrant with a sweet berry taste.' },
  { id: 5, name: 'Pineapple', price: 3.00, unit: 'each', emoji: '🍍', description: 'Tropical and refreshing with a tangy bite.' },
  { id: 6, name: 'Watermelon', price: 4.00, unit: 'whole', emoji: '🍉', description: 'Hydrating and juicy, great for summer.' },
  { id: 7, name: 'Grapes', price: 2.20, unit: 'bunch', emoji: '🍇', description: 'Sweet, plump grapes in a colorful bunch.' },
  { id: 8, name: 'Mango', price: 2.80, unit: 'each', emoji: '🥭', description: 'A rich tropical fruit with smooth texture.' },
  { id: 9, name: 'Peach', price: 1.85, unit: 'each', emoji: '🍑', description: 'Velvety skin and a delicate sweet flavor.' },
  { id: 10, name: 'Pear', price: 1.65, unit: 'each', emoji: '🍐', description: 'Mild and elegant with a crisp finish.' }
];

const cartKey = 'fruit-shop-cart';

function getCart() {
  const saved = localStorage.getItem(cartKey);
  return saved ? JSON.parse(saved) : [];
}

function saveCart(cart) {
  localStorage.setItem(cartKey, JSON.stringify(cart));
}

function addToCart(productId) {
  const qtyInput = document.querySelector(`#qty-${productId}`);
  const quantity = Number(qtyInput?.value || 1);
  const cart = getCart();
  const existing = cart.find(item => item.id === productId);

  if (existing) existing.quantity += quantity;
  else cart.push({ id: productId, quantity });

  saveCart(cart);
  window.location.href = 'shopping-cart.html';
}

function renderProducts() {
  const list = document.getElementById('product-list');
  if (!list) return;

  list.innerHTML = products.map(product => `
    <article class="product-card">
      <div class="product-emoji">${product.emoji}</div>
      <h2>${product.name}</h2>
      <p>${product.description}</p>
      <div class="product-price">$${product.price.toFixed(2)} / ${product.unit}</div>
      <label>Qty:
        <input class="qty-input" id="qty-${product.id}" type="number" min="1" value="1" />
      </label>
      <p>
        <button class="button" onclick="addToCart(${product.id})">Add to Cart</button>
        <a class="link-button" href="product-details.html?id=${product.id}">Details</a>
      </p>
    </article>
  `).join('');
}

function renderDetails() {
  const detailRoot = document.getElementById('product-details-content');
  if (!detailRoot) return;

  const params = new URLSearchParams(window.location.search);
  const productId = Number(params.get('id') || 1);
  const product = products.find(item => item.id === productId) || products[0];

  detailRoot.innerHTML = `
    <article class="detail-card">
      <div class="product-emoji">${product.emoji}</div>
      <h2>${product.name}</h2>
      <p>${product.description}</p>
      <div class="product-price">$${product.price.toFixed(2)} / ${product.unit}</div>
      <button class="button" onclick="addToCart(${product.id})">Add to Cart</button>
    </article>
  `;
}

function renderCart() {
  const cartRoot = document.getElementById('cart-content');
  if (!cartRoot) return;

  const cart = getCart();
  if (!cart.length) {
    cartRoot.innerHTML = '<div class="cart-item">Your cart is empty.</div>';
    return;
  }

  const cartItems = cart.map(item => {
    const product = products.find(p => p.id === item.id);
    const total = product.price * item.quantity;
    return `
      <article class="cart-item">
        <h3>${product.name}</h3>
        <p>Qty: <input class="qty-update" type="number" min="1" value="${item.quantity}" data-id="${item.id}" /></p>
        <p>Total: $${total.toFixed(2)}</p>
        <button class="remove-button" onclick="removeFromCart(${item.id})">Remove</button>
      </article>
    `;
  }).join('');

  const subtotal = cart.reduce((sum, item) => {
    const product = products.find(p => p.id === item.id);
    return sum + (product.price * item.quantity);
  }, 0);

  cartRoot.innerHTML = `
    <div>${cartItems}</div>
    <div class="cart-summary">
      <strong>Subtotal: $${subtotal.toFixed(2)}</strong>
      <p><a class="button" href="checkout.html">Proceed to Checkout</a></p>
    </div>
  `;

  cartRoot.querySelectorAll('.qty-update').forEach(input => {
    input.addEventListener('change', (event) => {
      const id = Number(event.target.dataset.id);
      const value = Math.max(1, Number(event.target.value || 1));
      const cart = getCart();
      const item = cart.find(entry => entry.id === id);
      if (item) item.quantity = value;
      saveCart(cart);
      renderCart();
    });
  });
}

function removeFromCart(productId) {
  const cart = getCart().filter(item => item.id !== productId);
  saveCart(cart);
  renderCart();
}

function renderCheckout() {
  const checkoutRoot = document.getElementById('checkout-content');
  if (!checkoutRoot) return;

  const cart = getCart();
  if (!cart.length) {
    checkoutRoot.innerHTML = '<div class="checkout-item">Your cart is empty.</div>';
    return;
  }

  const items = cart.map(item => {
    const product = products.find(p => p.id === item.id);
    return `
      <article class="checkout-item">
        <h3>${product.name}</h3>
        <p>Qty: ${item.quantity} | Price: $${product.price.toFixed(2)}</p>
      </article>
    `;
  }).join('');

  const total = cart.reduce((sum, item) => {
    const product = products.find(p => p.id === item.id);
    return sum + (product.price * item.quantity);
  }, 0);

  checkoutRoot.innerHTML = `
    <div>${items}</div>
    <div class="checkout-summary">
      <strong>Total: $${total.toFixed(2)}</strong>
      <p><button class="button" onclick="processOrder()">Process Order</button></p>
    </div>
  `;
}

function processOrder() {
  localStorage.removeItem(cartKey);
  alert('Order processed successfully.');
  window.location.href = 'index.html';
}

function initApp() {
  renderProducts();
  renderDetails();
  renderCart();
  renderCheckout();
}

window.addEventListener('DOMContentLoaded', initApp);
