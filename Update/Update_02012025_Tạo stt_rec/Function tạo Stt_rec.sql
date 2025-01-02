-- Tạo SEQUENCE
CREATE SEQUENCE seq_stt_rec_code
START WITH 1
INCREMENT BY 1
NO MINVALUE
NO MAXVALUE
CACHE 1;

-- Tạo hàm để sinh mã
CREATE OR REPLACE FUNCTION generate_stt_rec(ma_dvcs varchar(2), ma_ct ud_char3) 
RETURNS TEXT AS $$
BEGIN
    RETURN ma_dvcs||'-'|| LPAD(nextval('seq_stt_rec_code')::TEXT, 15, '0')||ma_ct;
END;
$$ LANGUAGE plpgsql;

select * from generate_stt_rec('AB','HDA')

SELECT setval('seq_stt_rec_code', 1, false); // Set lại giá trị sequence về bằng 1