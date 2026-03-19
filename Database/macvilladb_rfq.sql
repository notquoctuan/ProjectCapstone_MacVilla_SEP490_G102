-- 1. Thêm các cột cho bảng rfq
ALTER TABLE rfq
ADD COLUMN rfq_code VARCHAR(50) NULL,
ADD COLUMN assigned_sale_id BIGINT NULL,
ADD COLUMN customer_name VARCHAR(255) NULL,
ADD COLUMN company_name VARCHAR(255) NULL,
ADD COLUMN phone VARCHAR(50) NULL,
ADD COLUMN email VARCHAR(255) NULL,
ADD COLUMN address VARCHAR(500) NULL,
ADD COLUMN project_name VARCHAR(255) NULL,
ADD COLUMN expected_delivery_date DATETIME NULL,
ADD COLUMN priority VARCHAR(20) NULL,
ADD COLUMN internal_note TEXT NULL,
ADD COLUMN updated_at DATETIME DEFAULT CURRENT_TIMESTAMP NULL;

ALTER TABLE rfq
ADD CONSTRAINT rfq_ibfk_2 FOREIGN KEY (assigned_sale_id) REFERENCES users(user_id) ON DELETE SET NULL;

-- 2. Thêm các cột cho bảng quotations
ALTER TABLE quotations
ADD COLUMN quotation_code VARCHAR(50) NULL,
ADD COLUMN created_by BIGINT NULL,
ADD COLUMN sub_total DECIMAL(15,2) NULL,
ADD COLUMN discount_total DECIMAL(15,2) NULL,
ADD COLUMN vat_rate DECIMAL(5,2) NULL,
ADD COLUMN vat_amount DECIMAL(15,2) NULL,
ADD COLUMN total_amount DECIMAL(15,2) NULL,
ADD COLUMN internal_note TEXT NULL,
ADD COLUMN reject_reason TEXT NULL,
ADD COLUMN updated_at DATETIME DEFAULT CURRENT_TIMESTAMP NULL,
ADD COLUMN sent_at DATETIME NULL;

ALTER TABLE quotations
ADD CONSTRAINT quotations_ibfk_2 FOREIGN KEY (created_by) REFERENCES users(user_id) ON DELETE SET NULL;

-- 3. Tạo bảng rfq_items
CREATE TABLE rfq_items (
    rfq_item_id BIGINT AUTO_INCREMENT PRIMARY KEY,
    rfq_id BIGINT NOT NULL,
    product_id BIGINT NULL,
    sku VARCHAR(100) NULL,
    product_name VARCHAR(255) NULL,
    quantity INT NOT NULL,
    unit VARCHAR(50) NULL,
    note TEXT NULL,
    CONSTRAINT rfq_items_ibfk_1 FOREIGN KEY (rfq_id) REFERENCES rfq(rfq_id) ON DELETE CASCADE,
    CONSTRAINT rfq_items_ibfk_2 FOREIGN KEY (product_id) REFERENCES products(product_id) ON DELETE SET NULL
);

-- 4. Tạo bảng quotation_items
CREATE TABLE quotation_items (
    quotation_item_id BIGINT AUTO_INCREMENT PRIMARY KEY,
    quotation_id BIGINT NOT NULL,
    product_id BIGINT NULL,
    sku VARCHAR(100) NULL,
    product_name VARCHAR(255) NULL,
    quantity INT NOT NULL,
    unit VARCHAR(50) NULL,
    unit_price DECIMAL(15,2) NOT NULL,
    discount_percent DECIMAL(5,2) NOT NULL,
    line_total DECIMAL(15,2) NOT NULL,
    CONSTRAINT quotation_items_ibfk_1 FOREIGN KEY (quotation_id) REFERENCES quotations(quotation_id) ON DELETE CASCADE,
    CONSTRAINT quotation_items_ibfk_2 FOREIGN KEY (product_id) REFERENCES products(product_id) ON DELETE SET NULL
);
