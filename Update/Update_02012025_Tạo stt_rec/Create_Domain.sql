

CREATE DOMAIN email_domain AS VARCHAR(255)
    CONSTRAINT valid_email CHECK (
        VALUE ~ '^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$'
    );

CREATE DOMAIN ud_positive_integer AS INTEGER
    CONSTRAINT positive_check CHECK (VALUE > 0);

CREATE DOMAIN ud_ma as varchar(50)

CREATE DOMAIN ud_id AS VARCHAR(50)
    CONSTRAINT not_empty CHECK (VALUE IS NOT NULL AND LENGTH(VALUE) > 0);

CREATE DOMAIN ud_name AS VARCHAR(500)
    CONSTRAINT not_empty CHECK (VALUE IS NOT NULL AND LENGTH(VALUE) > 0);

Create domain ud_date as Date constraint valid_date check (value >'1900-01-01' and value <='2999-12-31') ;

CREATE DOMAIN ud_time AS TIME
    DEFAULT '00:00:00'
    CONSTRAINT valid_time CHECK (VALUE >= '00:00:00' AND VALUE <= '23:59:59');

CREATE DOMAIN ud_work_time AS TIME
    CONSTRAINT work_hours CHECK (VALUE >= '08:00:00' AND VALUE <= '18:00:00');

CREATE DOMAIN ud_datetimez AS TIMESTAMPTZ
    DEFAULT CURRENT_TIMESTAMP
    CONSTRAINT valid_datetimez CHECK (VALUE >= '1900-01-01 00:00:00+00' AND VALUE <= '2100-12-31 23:59:59+00');

CREATE DOMAIN ud_memo as TEXT CONSTRAINT valid_length CHECK (length(value)>=0 and length (value)<=5000);

Create domain ud_char25 as varchar(25) Constraint valid_length CHECK (length(value)>=0 and length(value)<=25);

Create domain ud_int as Integer   DEFAULT 0  	

Create domain ud_smallint as smallint constraint valid_smallint CHECK (value between -32768 and 32768);

CREATE DOMAIN ud_bit AS BIT
    CONSTRAINT valid_bit CHECK (VALUE IN ('0', '1'));

CREATE DOMAIN ud_booleannotnull AS BOOLEAN
    DEFAULT FALSE
    CONSTRAINT valid_boolean CHECK (VALUE IN (TRUE, FALSE));

CREATE DOMAIN ud_boolean AS BOOLEAN
    CONSTRAINT valid_boolean CHECK (VALUE IN (TRUE, FALSE));


CREATE DOMAIN ud_double_float AS DOUBLE PRECISION
	Default 0.000
    CONSTRAINT valid_float CHECK (VALUE BETWEEN -1e100 AND 1e100);


CREATE DOMAIN ud_float AS REAL
    DEFAULT 0.0
    CONSTRAINT valid_float CHECK (VALUE BETWEEN -1e10 AND 1e10);
COMMENT ON DOMAIN ud_float IS 'float type stores up to 6 decimal places';


CREATE DOMAIN ud_vnd AS NUMERIC(15, 0)
    DEFAULT 0
    CONSTRAINT udvalid_vnd CHECK (VALUE >= 0);

CREATE DOMAIN ud_USD AS NUMERIC(15, 3)
    DEFAULT 0.000
    CONSTRAINT udvalid_vnd CHECK (VALUE >= 0);

CREATE DOMAIN ud_JPY AS NUMERIC(15, 3)
    DEFAULT 0.000
    CONSTRAINT udvalid_vnd CHECK (VALUE >= 0);

CREATE DOMAIN ud_CNY AS NUMERIC(15, 3)
    DEFAULT 0.000
    CONSTRAINT udvalid_vnd CHECK (VALUE >= 0);

Create domain ud_char3 as varchar(3) Constraint valid_length CHECK (length(value)>=0 and length(value)<=3);

select * from  gen_random_uuid()

Create domain ud_status varchar(1)

	
CREATE DOMAIN ud_ip AS INET;


CREATE DOMAIN ud_uuid AS UUID;

CREATE DOMAIN ud_status AS smallint
	default 0;


