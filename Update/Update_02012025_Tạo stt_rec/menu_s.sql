-- Table: public.menu_s

-- DROP TABLE IF EXISTS public.menu_s;

CREATE TABLE IF NOT EXISTS public.menu_s
(
    menu_scode integer NOT NULL,
    menu_mcode integer NOT NULL,
    menu_sname character varying COLLATE pg_catalog."default" NOT NULL,
    sort_no integer,
    form_name character varying COLLATE pg_catalog."default",
    use_ck character varying(1) COLLATE pg_catalog."default" DEFAULT 'Y'::character varying,
    user_id0 character varying(15) COLLATE pg_catalog."default",
    user_id2 character varying(15) COLLATE pg_catalog."default",
    created_at timestamp without time zone NOT NULL,
    updated_at timestamp without time zone,
    deleted_at timestamp without time zone,
    CONSTRAINT menus_pkey PRIMARY KEY (menu_scode),
    CONSTRAINT fk_menum FOREIGN KEY (menu_scode)
        REFERENCES public.menu_m (menu_mcode) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS public.menu_s
    OWNER to postgres;