-- Script để thêm column gift_code_id vào bảng user_milestone_claims

-- Thêm column gift_code_id (nullable)
ALTER TABLE pico.user_milestone_claims 
ADD COLUMN gift_code_id BIGINT(20) UNSIGNED NULL 
COMMENT 'ID của gift code được tạo khi claim milestone'
AFTER claimed_at;

-- Thêm foreign key constraint (optional)
ALTER TABLE pico.user_milestone_claims 
ADD CONSTRAINT fk_user_milestone_claims_gift_code_id 
FOREIGN KEY (gift_code_id) REFERENCES pico.gift_codes(id) 
ON DELETE SET NULL 
ON UPDATE CASCADE;

-- Thêm index cho performance
CREATE INDEX idx_user_milestone_claims_gift_code_id 
ON pico.user_milestone_claims(gift_code_id);
