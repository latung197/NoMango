-- Table: public.menu_m

-- DROP TABLE IF EXISTS public.menu_m;

CREATE TABLE IF NOT EXISTS public.menu_m
(
    menu_mcode integer NOT NULL,
    menu_mname character varying COLLATE pg_catalog."default" NOT NULL,
    sort_no integer,
    use_ck character varying(1) COLLATE pg_catalog."default" DEFAULT 'Y'::character varying,
    user_id0 character varying(15) COLLATE pg_catalog."default",
    user_id2 character varying(15) COLLATE pg_catalog."default",
    created_at timestamp without time zone NOT NULL,
    updated_at timestamp without time zone,
    deleted_at timestamp without time zone,
    CONSTRAINT menum_pkey PRIMARY KEY (menu_mcode)
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS public.menu_m
    OWNER to postgres;