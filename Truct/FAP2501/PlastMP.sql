--
-- PostgreSQL database dump
--

-- Dumped from database version 15.7
-- Dumped by pg_dump version 16.3

-- Started on 2025-07-15 15:47:43

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

--
-- TOC entry 251 (class 1255 OID 16182742)
-- Name: listweeks(character varying, character varying); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.listweeks(_month character varying, _week_start character varying) RETURNS TABLE(week_id bigint, week_name text, week_start date, week_end date)
    LANGUAGE plpgsql
    AS $$
DECLARE
    
BEGIN
	-- Return List Week Of Month
	RETURN QUERY
	SELECT * FROM (
		SELECT ROW_NUMBER() OVER(ORDER BY date_trunc('week', day + interval '1 day') - interval '1 day') as week_id, 
				'第'||ROW_NUMBER() OVER(ORDER BY (date_trunc('week', day + interval '1 day') - interval '1 day'))||'週目' week_name , 
				(date_trunc('week', day + interval '1 day') - interval '1 day')::date  AS week_start, 
				(date_trunc('week', day + interval '1 day') - interval '1 day')::date +6 week_end
		FROM generate_series(_month::Date, _month::Date + interval '1 month - 1 day', interval '1 day') day
		GROUP BY date_trunc('week', day + interval '1 day') - interval '1 day'
		ORDER BY week_start) A 
	WHERE CASE WHEN (_week_start = '' ) THEN 1=1 ELSE A.week_start = _week_start::date END;
	
END
$$;


ALTER FUNCTION public.listweeks(_month character varying, _week_start character varying) OWNER TO postgres;

--
-- TOC entry 253 (class 1255 OID 16443715)
-- Name: machinedatabymonth(character varying, character varying); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.machinedatabymonth(_month character varying, _machine character varying) RETURNS TABLE(date date, "設備稼働時間" integer, "生産件数" integer, equipment_operation_hours1 character varying)
    LANGUAGE plpgsql
    AS $$
DECLARE
    _month_start date;
    _month_end date;
BEGIN
	    -- Convert '202504' to '2025-04-01'
	    _month_start := to_date(_month, 'YYYY/MM');
	    -- Day Last Month
	    _month_end := (_month_start + interval '1 month' - interval '1 day')::date;
		-- Create Temp Tale List Day Of Month
		DROP TABLE IF EXISTS TEMP_MONTH;
		CREATE TEMP TABLE TEMP_MONTH AS 
		SELECT generate_series(_month_start, _month_end,interval '1 day')::date AS dateweek ,
				''ACHIEVEMENT_REGISTRATION_DATE,
				0 EQUIPMENT_OPERATION_HOURS,
				0 PRODUCTION_COUNT ;

		-- Get Data In Month
		DROP TABLE IF EXISTS TEMP_DATAMACHINE;
		CREATE TEMP TABLE TEMP_DATAMACHINE AS
		SELECT "ACHIEVEMENT_REGISTRATION_DATE" , 
				"EQUIPMENT_OPERATION_HOURS",
				"PRODUCTION_COUNT" 
		FROM "TRN_OPERATION_OEE" 
		WHERE "MACHINE_NO" = _machine
			AND "ACHIEVEMENT_REGISTRATION_DATE" BETWEEN _month_start AND _month_end;

		-- Update Data To Tale List Day Of Month
		UPDATE TEMP_MONTH a SET 
				PRODUCTION_COUNT = b."PRODUCTION_COUNT",
				EQUIPMENT_OPERATION_HOURS = b."EQUIPMENT_OPERATION_HOURS"
			FROM TEMP_DATAMACHINE b WHERE a.dateweek = b."ACHIEVEMENT_REGISTRATION_DATE";

		-- Return
		RETURN QUERY 
		SELECT dateweek as Date, 
				EQUIPMENT_OPERATION_HOURS as 設備稼働時間,
				PRODUCTION_COUNT as 生産件数,
			 TO_CHAR(
        make_interval(secs => EQUIPMENT_OPERATION_HOURS), 
        'HH24:MI:SS'
    ):: Character varying AS EQUIPMENT_OPERATION_HOURS1
		FROM TEMP_MONTH ORDER BY date;
		DROP TABLE IF EXISTS TEMP_MONTH;
		DROP TABLE IF EXISTS TEMP_DATAMACHINE;
END
$$;


ALTER FUNCTION public.machinedatabymonth(_month character varying, _machine character varying) OWNER TO postgres;

--
-- TOC entry 252 (class 1255 OID 16358626)
-- Name: machinedatabyweek(character varying, character varying, character varying); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.machinedatabyweek(_week_start character varying, _week_end character varying, _machine character varying) RETURNS TABLE(date date, "時間稼働率" numeric, "生産件数" integer, "加工時間" integer, "設備稼働時間" integer, "負荷時間" integer, "ロス停止時間" integer)
    LANGUAGE plpgsql
    AS $$
DECLARE
   
BEGIN
		-- Create Temp Table List Day of Week
		DROP TABLE IF EXISTS TEMP_WEEK;
		CREATE TEMP TABLE TEMP_WEEK AS 
		SELECT generate_series(_week_start::date, _week_end::date,interval '1 day')::date AS dateweek ,
				''ACHIEVEMENT_REGISTRATION_DATE,
				0::numeric(10,2) TIME_OPERATING_RATE, 
				0 PRODUCTION_COUNT , 
				0 PROCESSING_TIME, 
				0 EQUIPMENT_OPERATION_HOURS, 
				0 LOAD_TIME, 
				0 LOSS_STOP_TIME;
		-- Get Data Machine in Week
		DROP TABLE IF EXISTS TEMP_DATAMACHINE;
		CREATE TEMP TABLE TEMP_DATAMACHINE AS
		SELECT "ACHIEVEMENT_REGISTRATION_DATE", "TIME_OPERATING_RATE" , 
		"PRODUCTION_COUNT" , "PROCESSING_TIME",
		"EQUIPMENT_OPERATION_HOURS" , 
		"LOAD_TIME" ,"LOSS_STOP_TIME"  
		FROM "TRN_OPERATION_OEE" 
		WHERE "MACHINE_NO" = _machine
			AND "ACHIEVEMENT_REGISTRATION_DATE" BETWEEN _week_start::DATE AND _week_end::DATE;
		-- Update Data To Tale List Day Of Week
		UPDATE TEMP_WEEK a SET 
			TIME_OPERATING_RATE = b."TIME_OPERATING_RATE",
			PRODUCTION_COUNT = b."PRODUCTION_COUNT",
			PROCESSING_TIME = b."PROCESSING_TIME",
			EQUIPMENT_OPERATION_HOURS = b."EQUIPMENT_OPERATION_HOURS",
			LOAD_TIME = b."LOAD_TIME",
			LOSS_STOP_TIME = b."LOSS_STOP_TIME"
			FROM TEMP_DATAMACHINE b WHERE a.dateweek = b."ACHIEVEMENT_REGISTRATION_DATE";
		-- Return
		RETURN QUERY 
		SELECT dateweek as Date, 
				TIME_OPERATING_RATE as 時間稼働率, 
				PRODUCTION_COUNT as 生産件数, 
				PROCESSING_TIME as 加工時間, 
				EQUIPMENT_OPERATION_HOURS as 設備稼働時間, 
				LOAD_TIME as 負荷時間, 
				LOSS_STOP_TIME as ロス停止時間
		FROM TEMP_WEEK;
		DROP TABLE IF EXISTS TEMP_WEEK;
		DROP TABLE IF EXISTS TEMP_DATAMACHINE;
END
$$;


ALTER FUNCTION public.machinedatabyweek(_week_start character varying, _week_end character varying, _machine character varying) OWNER TO postgres;

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- TOC entry 229 (class 1259 OID 12231604)
-- Name: MST_FACTORY; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."MST_FACTORY" (
    "ID" bigint NOT NULL,
    "FACTORY_CD" integer NOT NULL,
    "FACTORY_NAME" character varying(20)
);


ALTER TABLE public."MST_FACTORY" OWNER TO postgres;

--
-- TOC entry 228 (class 1259 OID 12231603)
-- Name: MST_FACTORY_ID_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public."MST_FACTORY_ID_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public."MST_FACTORY_ID_seq" OWNER TO postgres;

--
-- TOC entry 3392 (class 0 OID 0)
-- Dependencies: 228
-- Name: MST_FACTORY_ID_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public."MST_FACTORY_ID_seq" OWNED BY public."MST_FACTORY"."ID";


--
-- TOC entry 231 (class 1259 OID 12231620)
-- Name: MST_MACHINE; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."MST_MACHINE" (
    "ID" bigint NOT NULL,
    "HMI_NO" character varying(5) NOT NULL,
    "MACHINE_NO" character varying(5) NOT NULL,
    "MACHINE_NAME" character varying(25),
    "INSTALLATION_LOCATION_CD" integer
);


ALTER TABLE public."MST_MACHINE" OWNER TO postgres;

--
-- TOC entry 230 (class 1259 OID 12231619)
-- Name: MST_MACHINE_ID_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public."MST_MACHINE_ID_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public."MST_MACHINE_ID_seq" OWNER TO postgres;

--
-- TOC entry 3393 (class 0 OID 0)
-- Dependencies: 230
-- Name: MST_MACHINE_ID_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public."MST_MACHINE_ID_seq" OWNED BY public."MST_MACHINE"."ID";


--
-- TOC entry 237 (class 1259 OID 13898262)
-- Name: TRN_IMPORT_HISTORY; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."TRN_IMPORT_HISTORY" (
    "ID" bigint NOT NULL,
    "FILENAME" character varying(255),
    "MACHINE_NO" character varying(5),
    "FACTORY_CD" integer,
    "RECORDCOUNT" integer,
    "STATUS" character varying(50),
    "NOTE" character varying(500),
    "IMPORTTIME" timestamp without time zone,
    "FLAG" character varying(1)
);


ALTER TABLE public."TRN_IMPORT_HISTORY" OWNER TO postgres;

--
-- TOC entry 239 (class 1259 OID 15935315)
-- Name: TRN_IMPORT_HISTORY_DETAIL; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."TRN_IMPORT_HISTORY_DETAIL" (
    "ID" bigint NOT NULL,
    "IDDATA" bigint,
    "FACTORY_CD" bigint,
    "MACHINE_NO" character varying(5),
    "FILENAME" character varying(250),
    "IMPORTTIME" timestamp without time zone
);


ALTER TABLE public."TRN_IMPORT_HISTORY_DETAIL" OWNER TO postgres;

--
-- TOC entry 238 (class 1259 OID 15935314)
-- Name: TRN_IMPORT_HISTORY_DETAIL_ID_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public."TRN_IMPORT_HISTORY_DETAIL_ID_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public."TRN_IMPORT_HISTORY_DETAIL_ID_seq" OWNER TO postgres;

--
-- TOC entry 3394 (class 0 OID 0)
-- Dependencies: 238
-- Name: TRN_IMPORT_HISTORY_DETAIL_ID_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public."TRN_IMPORT_HISTORY_DETAIL_ID_seq" OWNED BY public."TRN_IMPORT_HISTORY_DETAIL"."ID";


--
-- TOC entry 236 (class 1259 OID 13898261)
-- Name: TRN_IMPORT_HISTORY_ID_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public."TRN_IMPORT_HISTORY_ID_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public."TRN_IMPORT_HISTORY_ID_seq" OWNER TO postgres;

--
-- TOC entry 3395 (class 0 OID 0)
-- Dependencies: 236
-- Name: TRN_IMPORT_HISTORY_ID_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public."TRN_IMPORT_HISTORY_ID_seq" OWNED BY public."TRN_IMPORT_HISTORY"."ID";


--
-- TOC entry 233 (class 1259 OID 12231661)
-- Name: TRN_OPERATION_OEE; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."TRN_OPERATION_OEE" (
    "ID" bigint NOT NULL,
    "FACTORY_CD" integer NOT NULL,
    "SHIFT_ID" integer,
    "LINE_ID" integer,
    "PROCESS_ID" integer,
    "MACHINE_NO" character varying(5) NOT NULL,
    "ACHIEVEMENT_REGISTRATION_DATE" date,
    "ACHIEVEMENT_REGISTRATION_TIME" timestamp without time zone,
    "PROCESSING_TIME" integer,
    "PROCESSING_STOP_TIME" integer,
    "LOSS_STOP_TIME" integer,
    "PRODUCTION_COUNT" integer,
    "OPERATION_RATE" numeric(10,2),
    "EQUIPMENT_OPERATION_HOURS" integer,
    "LOAD_TIME" integer,
    "TIME_OPERATING_RATE" numeric(10,2)
);


ALTER TABLE public."TRN_OPERATION_OEE" OWNER TO postgres;

--
-- TOC entry 232 (class 1259 OID 12231660)
-- Name: TRN_OPERATION_OEE_ID_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public."TRN_OPERATION_OEE_ID_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public."TRN_OPERATION_OEE_ID_seq" OWNER TO postgres;

--
-- TOC entry 3396 (class 0 OID 0)
-- Dependencies: 232
-- Name: TRN_OPERATION_OEE_ID_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public."TRN_OPERATION_OEE_ID_seq" OWNED BY public."TRN_OPERATION_OEE"."ID";


--
-- TOC entry 235 (class 1259 OID 12231668)
-- Name: TRN_OPERATION_RESULTS; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."TRN_OPERATION_RESULTS" (
    "ID" bigint NOT NULL,
    "FACTORY_CD" integer NOT NULL,
    "SHIFT_ID" integer,
    "LINE_ID" integer,
    "PROCESS_ID" integer,
    "MACHINE_NO" character varying(5) NOT NULL,
    "ACHIEVEMENT_REGISTRATION_DATE" date,
    "ACHIEVEMENT_REGISTRATION_TIME" timestamp without time zone,
    "SLIP_NO" character varying(10),
    "CUSTOMER_CD" character varying(10),
    "CUSTOMER_NAME" character varying(50),
    "DUE_DATE" date,
    "PRODUCT_NAME_1" character varying(50),
    "PRODUCT_NAME_2" character varying(50),
    "VALUE" integer,
    "OPERATOR_1" character varying(50),
    "OPERATOR_2" character varying(50),
    "OPERATOR_3" character varying(50),
    "PROCESSING_START_TIME" timestamp without time zone,
    "PROCESSING_END_TIME" timestamp without time zone,
    "PROCESSING_TIME" integer,
    "PROGRESS_RATE" character varying(5),
    "STANDARD_TIME" integer,
    "PROCESSING_STOP_TIME" integer,
    "LOSS_STOP_TIME" integer,
    "MEASUREMENT_INSPECTION" integer,
    "CHANGEOVER" integer,
    "CAD" integer,
    "EQUIPMENT_FAILURE" integer,
    "CLEANING" integer,
    "REST_TIME" integer
);


ALTER TABLE public."TRN_OPERATION_RESULTS" OWNER TO postgres;

--
-- TOC entry 234 (class 1259 OID 12231667)
-- Name: TRN_OPERATION_RESULTS_ID_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public."TRN_OPERATION_RESULTS_ID_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public."TRN_OPERATION_RESULTS_ID_seq" OWNER TO postgres;

--
-- TOC entry 3397 (class 0 OID 0)
-- Dependencies: 234
-- Name: TRN_OPERATION_RESULTS_ID_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public."TRN_OPERATION_RESULTS_ID_seq" OWNED BY public."TRN_OPERATION_RESULTS"."ID";


--
-- TOC entry 3215 (class 2604 OID 12231607)
-- Name: MST_FACTORY ID; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."MST_FACTORY" ALTER COLUMN "ID" SET DEFAULT nextval('public."MST_FACTORY_ID_seq"'::regclass);


--
-- TOC entry 3216 (class 2604 OID 12231623)
-- Name: MST_MACHINE ID; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."MST_MACHINE" ALTER COLUMN "ID" SET DEFAULT nextval('public."MST_MACHINE_ID_seq"'::regclass);


--
-- TOC entry 3219 (class 2604 OID 13898265)
-- Name: TRN_IMPORT_HISTORY ID; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."TRN_IMPORT_HISTORY" ALTER COLUMN "ID" SET DEFAULT nextval('public."TRN_IMPORT_HISTORY_ID_seq"'::regclass);


--
-- TOC entry 3220 (class 2604 OID 15935318)
-- Name: TRN_IMPORT_HISTORY_DETAIL ID; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."TRN_IMPORT_HISTORY_DETAIL" ALTER COLUMN "ID" SET DEFAULT nextval('public."TRN_IMPORT_HISTORY_DETAIL_ID_seq"'::regclass);


--
-- TOC entry 3217 (class 2604 OID 12231664)
-- Name: TRN_OPERATION_OEE ID; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."TRN_OPERATION_OEE" ALTER COLUMN "ID" SET DEFAULT nextval('public."TRN_OPERATION_OEE_ID_seq"'::regclass);


--
-- TOC entry 3218 (class 2604 OID 12231671)
-- Name: TRN_OPERATION_RESULTS ID; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."TRN_OPERATION_RESULTS" ALTER COLUMN "ID" SET DEFAULT nextval('public."TRN_OPERATION_RESULTS_ID_seq"'::regclass);


--
-- TOC entry 3376 (class 0 OID 12231604)
-- Dependencies: 229
-- Data for Name: MST_FACTORY; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."MST_FACTORY" ("ID", "FACTORY_CD", "FACTORY_NAME") FROM stdin;
1	1	Nhà máy 1
2	2	Nhà máy 2
\.


--
-- TOC entry 3378 (class 0 OID 12231620)
-- Dependencies: 231
-- Data for Name: MST_MACHINE; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."MST_MACHINE" ("ID", "HMI_NO", "MACHINE_NO", "MACHINE_NAME", "INSTALLATION_LOCATION_CD") FROM stdin;
10	HMI10	10	NakamuraTome_SC260-2	1
11	HMI11	11	NakamuraTome_SC250	2
12	HMI12	12	Takamatsu	1
13	HMI13	13	RoboDrill1	1
14	HMI14	14	RoboDrill2	1
15	HMI15	15	RoboDrill3	1
16	HMI16	16	RoboDrill4	1
17	HMI17	17	RoboDrill5	1
18	HMI18	18	RoboDrill6	1
19	HMI19	19	NC-Rooter1	2
20	HMI20	20	NC-Rooter2	2
21	HMI21	21	NC-Rooter3	2
1	HMI1	01	NV4000	1
2	HMI2	02	NV5000	2
3	HMI3	03	Enshu450FV	1
4	HMI4	04	Enshu450V	1
5	HMI5	05	Enshu1	1
6	HMI6	06	Enshu2	2
7	HMI7	07	Yamazaki-SEV320NCR	1
9	HMI9	09	NakamuraTome_SC260-1	1
8	HMI8	08	NakamuraTome_TMC-15ll	2
\.


--
-- TOC entry 3384 (class 0 OID 13898262)
-- Dependencies: 237
-- Data for Name: TRN_IMPORT_HISTORY; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."TRN_IMPORT_HISTORY" ("ID", "FILENAME", "MACHINE_NO", "FACTORY_CD", "RECORDCOUNT", "STATUS", "NOTE", "IMPORTTIME", "FLAG") FROM stdin;
1150	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 07:54:11.77393	0
1151	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 07:55:11.860694	0
1152	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 07:56:11.964994	0
1153	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 07:57:12.06038	0
1154	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 07:58:12.182779	0
1165	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 08:09:13.270809	0
1166	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 08:10:13.341818	0
1167	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 08:11:13.416495	0
702	1_NV4000_202503.CSV	1	1	21	成功	ファイルのデータは上書きされました。	2025-04-23 10:32:31.85217	1
724	1_NV4000_202501.CSV	1	1	23	成功	ファイルのデータは上書きされました。	2025-04-28 16:45:20.429301	1
725	1_NV4000_202501.CSV	1	1	23	成功	ファイルのデータは上書きされました。	2025-04-28 16:48:58.650583	1
727	3_Enshu450FV_202501.CSV	3	1	23	成功	ファイルのデータは上書きされました。	2025-04-28 17:46:28.38795	1
741	14_RoboDrill2_202501.CSV	14	1	23	成功	データのインポートに成功しました。	2025-04-29 11:37:59.203864	1
742	15_RoboDrill3_202501.CSV	15	1	23	成功	データのインポートに成功しました。	2025-04-29 11:38:09.424373	1
743	16_RoboDrill4_202501.CSV	16	1	23	成功	データのインポートに成功しました。	2025-04-29 11:38:24.946914	1
744	17_RoboDrill5_202501.CSV	17	1	23	成功	データのインポートに成功しました。	2025-04-29 11:38:34.792155	1
745	18_RoboDrill6_202501.CSV	18	1	23	成功	データのインポートに成功しました。	2025-04-29 11:38:43.727986	1
746	19_NC-Rooter1_202501.CSV	19	2	23	成功	データのインポートに成功しました。	2025-04-29 11:38:55.161537	1
750	31_NV4000_202504.CSV		0	\N	失敗	設備Noが存在しない為、データのインポートに失敗しました。	2025-04-29 16:43:15.431325	1
751	DataLog_HMI41_20250409.csv		-1	\N	失敗	設備Noが存在しない為、データのインポートに失敗しました。	2025-04-29 16:43:56.361778	0
1168	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 08:12:13.611004	0
748	21_NC-Rooter3_202501.CSV	21	2	23	成功	ファイルのデータは上書きされました。	2025-04-29 11:39:12.372955	1
1169	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 08:13:13.716237	0
1186	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 08:30:15.063964	0
1187	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 08:31:15.200714	0
1188	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 08:32:15.24949	0
1189	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 08:33:15.323497	0
1190	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 08:34:15.370919	0
1201	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 08:45:16.332739	0
1202	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 08:46:16.497595	0
1203	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 08:47:16.604586	0
1204	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 08:48:16.644467	0
710	2_NV5000_202504.CSV	2	2	15	成功	ファイルのデータは上書きされました。	2025-04-23 10:33:05.29617	1
721	2_NV5000_202503.CSV	2	2	21	成功	ファイルのデータは上書きされました。	2025-04-24 17:59:56.656425	1
1217	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 09:01:17.815958	0
1218	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 09:02:17.889322	0
1219	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 09:03:18.01909	0
1220	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 09:04:18.112117	0
1221	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 09:05:18.165341	0
1222	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 09:06:18.302536	0
1233	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 09:17:19.373328	0
1234	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 09:18:19.436363	0
1235	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 09:19:19.487932	0
1236	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 09:20:19.572948	0
1237	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 09:21:19.660518	0
1238	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 09:22:19.751471	0
692	1_NV4000_202503.CSV	1	1	21	成功	ファイルのデータは上書きされました。	2025-04-23 08:41:30.641725	1
697	1_NV4000_202504.CSV	1	1	14	成功	ファイルのデータは上書きされました。	2025-04-23 08:55:42.358459	1
703	1_NV4000_202504.CSV	1	1	14	成功	データのインポートに成功しました。	2025-04-23 10:32:31.858539	1
1155	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 07:59:12.259735	0
1156	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 08:00:12.448535	0
1157	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 08:01:12.521481	0
1158	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 08:02:12.602999	0
1159	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 08:03:12.701836	0
1170	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 08:14:13.799508	0
1171	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 08:15:13.872302	0
1172	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 08:16:13.94032	0
1173	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 08:17:14.007405	0
1174	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 08:18:14.090988	0
709	2_NV5000_202503.CSV	2	2	21	成功	ファイルのデータは上書きされました。	2025-04-23 10:33:05.258469	1
718	2_NV5000_202503.CSV	2	2	21	成功	ファイルのデータは上書きされました。	2025-04-23 10:41:48.706846	1
1175	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 08:19:14.169027	0
723	19_NC-Rooter1_202411.CSV	19	2	18	成功	データのインポートに成功しました。	2025-04-28 10:11:43.350282	1
726	2_NV5000_202501.CSV	2	2	23	成功	ファイルのデータは上書きされました。	2025-04-28 17:26:24.303407	1
730	3_Enshu450FV_202501.CSV	3	1	23	成功	データのインポートに成功しました。	2025-04-29 11:35:56.714247	1
732	5_Enshu1_202501.CSV	5	1	23	成功	データのインポートに成功しました。	2025-04-29 11:36:23.429637	1
733	6_Enshu2_202501.CSV	6	2	23	成功	データのインポートに成功しました。	2025-04-29 11:36:32.736456	1
734	7_Yamazaki-SEV320NCR_202501.CSV	7	1	23	成功	データのインポートに成功しました。	2025-04-29 11:36:41.735191	1
735	8_NakamuraTome_TMC-15ll_202501.CSV	8	2	23	成功	データのインポートに成功しました。	2025-04-29 11:36:55.047714	1
737	10_NakamuraTome_SC260-2_202501.CSV	10	1	23	成功	データのインポートに成功しました。	2025-04-29 11:37:15.184075	1
738	11_NakamuraTome_SC250_202501.CSV	11	2	23	成功	データのインポートに成功しました。	2025-04-29 11:37:26.41856	1
739	12_Takamatsu_202501.CSV	12	1	23	成功	データのインポートに成功しました。	2025-04-29 11:37:38.129324	1
740	13_RoboDrill1_202501.CSV	13	1	23	成功	データのインポートに成功しました。	2025-04-29 11:37:48.987225	1
1191	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 08:35:15.434837	0
1192	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 08:36:15.529445	0
728	1_NV4000_202501.CSV	1	1	23	成功	ファイルのデータは上書きされました。	2025-04-29 11:35:26.646709	1
754	21_NC-Rooter3_202501.CSV	21	2	23	成功	データのインポートに成功しました。	2025-04-29 16:46:57.458144	1
729	2_NV5000_202501.CSV	2	2	23	成功	ファイルのデータは上書きされました。	2025-04-29 11:35:46.638346	1
1193	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 08:37:15.619996	0
1194	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 08:38:15.706378	0
1195	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 08:39:15.780998	0
1205	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 08:49:16.716635	0
1206	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 08:50:16.769704	0
1223	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 09:07:18.414972	0
1224	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 09:08:18.51049	0
1225	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 09:09:18.581864	0
1226	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 09:10:18.681915	0
1227	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 09:11:18.767073	0
731	4_Enshu450V_202501.CSV	4	1	23	成功	ファイルのデータは上書きされました。	2025-04-29 11:36:14.877326	1
736	9_NakamuraTome_SC260-1_202501.CSV	9	1	23	成功	ファイルのデータは上書きされました。	2025-04-29 11:37:05.620033	1
720	1_NV4000_202503.CSV	1	1	22	成功	ファイルのデータは上書きされました。	2025-04-24 11:50:37.297063	1
701	DataLog_HMI2_20250417.csv	2	2	15	成功	ファイルのデータは上書きされました。	2025-04-23 09:15:48.886001	1
693	1_NV4000_202504.CSV	1	1	14	成功	ファイルのデータは上書きされました。	2025-04-23 08:41:30.64173	1
1160	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 08:04:12.800653	0
1161	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 08:05:12.890217	0
1162	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 08:06:12.974265	0
1163	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 08:07:13.01725	0
1164	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 08:08:13.17855	0
1176	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 08:20:14.269351	0
1177	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 08:21:14.340787	0
722	19_NC-Rooter1_202410.CSV	19	2	21	成功	ファイルのデータは上書きされました。	2025-04-28 10:11:27.694135	1
1178	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 08:22:14.405178	0
1179	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 08:23:14.496384	0
1180	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 08:24:14.567054	0
1181	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 08:25:14.652851	0
1182	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 08:26:14.724425	0
1183	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 08:27:14.807245	0
1184	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 08:28:14.853602	0
1185	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 08:29:14.950126	0
1196	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 08:40:15.874436	0
1197	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 08:41:15.971891	0
1198	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 08:42:16.053513	0
1199	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 08:43:16.157455	0
1200	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 08:44:16.270457	0
1207	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 08:51:16.844238	0
1208	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 08:52:16.973386	0
1209	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 08:53:17.059278	0
1210	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 08:54:17.155632	0
1211	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 08:55:17.242342	0
1212	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 08:56:17.333036	0
1213	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 08:57:17.512781	0
1214	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 08:58:17.617817	0
1215	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 08:59:17.673206	0
1216	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 09:00:17.733123	0
1228	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 09:12:18.849781	0
1229	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 09:13:18.939322	0
1230	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 09:14:19.008904	0
1231	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 09:15:19.126004	0
1232	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 09:16:19.239111	0
1239	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 09:23:19.821771	0
1240	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 09:24:19.882863	0
1241	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 09:25:19.936027	0
1242	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 09:26:20.042306	0
1243	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 09:27:20.108775	0
1244	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 09:28:20.178625	0
1245	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 09:29:20.26707	0
1246	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 09:30:20.354838	0
1247	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 09:31:20.428125	0
1248	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 09:32:20.522491	0
1249	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 09:33:20.562254	0
1250	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 09:34:20.634752	0
1251	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 09:35:20.719556	0
1252	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 09:36:20.791031	0
1253	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 09:37:20.880794	0
1254	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 09:38:20.986684	0
1255	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 09:39:21.059887	0
1256	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 09:40:21.181034	0
1257	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 09:41:21.268227	0
1258	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 09:42:21.361035	0
1259	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 09:43:21.47371	0
1260	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 09:44:21.517377	0
1261	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 09:45:21.597984	0
1262	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 09:46:21.685084	0
1263	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 09:47:21.748477	0
1264	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 09:48:21.851421	0
1265	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 09:49:21.939201	0
1266	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 09:50:22.04932	0
1267	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 09:51:22.136696	0
1268	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 09:52:22.178433	0
1269	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 09:53:22.282678	0
1270	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 09:54:22.409	0
1271	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 09:55:22.46504	0
1272	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 09:56:22.538323	0
1273	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 09:57:22.648284	0
1274	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 09:58:22.707349	0
1275	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 09:59:22.812314	0
1276	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 10:00:22.886675	0
1277	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 10:01:22.952503	0
1278	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 10:02:23.015012	0
1279	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 10:03:23.095388	0
1280	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 10:04:23.205921	0
1281	DataLog_HMI1_20250501.csv	1	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 10:05:23.270044	0
1282	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 10:06:23.340692	0
1283	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 10:07:23.46281	0
1284	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 10:08:23.614249	0
1285	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 10:09:23.690435	0
1286	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 10:10:23.807389	0
1287	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 10:11:23.891674	0
1288	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 10:12:23.997536	0
1289	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 10:13:24.114452	0
1290	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 10:14:24.19629	0
1291	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 10:15:24.274551	0
1292	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 10:16:24.375628	0
1293	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 10:17:24.456497	0
1294	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 10:18:24.545876	0
1295	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 10:19:24.638489	0
1296	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 10:20:24.735912	0
1297	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 10:21:24.828123	0
1298	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 10:22:24.919003	0
1299	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 10:23:25.014316	0
1300	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 10:24:25.096251	0
1301	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 10:25:25.154609	0
1302	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 10:26:25.254686	0
1303	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 10:27:25.361572	0
1304	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 10:28:25.448804	0
1305	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 10:29:25.547295	0
1306	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 10:30:25.629971	0
1307	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 10:31:25.721747	0
1308	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 10:32:25.807856	0
1309	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 10:33:25.908841	0
1310	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 10:34:25.951711	0
1311	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 10:35:26.023696	0
1312	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 10:36:26.111604	0
1313	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 10:37:26.202302	0
1314	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 10:38:26.278474	0
1315	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 10:39:26.315913	0
1316	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 10:40:26.423059	0
1317	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 10:41:26.503499	0
1318	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 10:42:26.664201	0
1319	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 10:43:26.766359	0
1320	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 10:44:26.887614	0
1321	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 10:45:26.956682	0
1322	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 10:46:27.090229	0
1323	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 10:47:27.232404	0
1324	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 10:48:27.337072	0
1325	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 10:49:27.425911	0
1326	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 10:50:27.512638	0
1327	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 10:51:27.572424	0
1328	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 10:52:27.658367	0
1329	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 10:53:27.727591	0
1330	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 10:54:27.848419	0
1331	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 10:55:27.925131	0
1332	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 10:56:28.00088	0
1333	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 10:57:28.077871	0
1334	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 10:58:28.147518	0
1335	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 10:59:28.263603	0
1336	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 11:00:28.383506	0
1337	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 11:01:28.441176	0
1338	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 11:02:28.927533	0
1339	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 11:04:52.08604	0
1340	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 11:05:52.258276	0
1341	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 11:06:52.355649	0
1342	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 11:07:52.543695	0
1343	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 11:08:52.634395	0
1344	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 11:09:52.762118	0
1345	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 11:10:52.815527	0
1346	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 11:11:52.939639	0
1347	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 11:12:53.024087	0
1348	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 11:13:53.124767	0
1349	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 11:14:53.261822	0
1350	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 11:15:53.383963	0
1351	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 11:16:53.490539	0
1352	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 11:17:53.571632	0
1353	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 11:18:53.651587	0
1354	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 11:20:19.401397	0
1355	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 11:21:19.537899	0
1356	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 11:22:19.64864	0
1357	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 11:23:19.739887	0
1358	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 11:24:19.787092	0
1359	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 11:25:19.97825	0
1360	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 11:26:20.054276	0
1361	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 11:27:20.163317	0
1362	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 11:28:20.208758	0
1363	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 11:29:20.367014	0
1364	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 11:30:20.444914	0
1365	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 11:31:20.549352	0
1366	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 11:32:20.693666	0
1367	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 11:33:20.816432	0
1368	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 11:34:20.863316	0
1369	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 11:35:20.980475	0
1370	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 11:36:21.088879	0
1371	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 11:37:21.192725	0
1372	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 11:38:21.251033	0
1373	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 11:39:21.36527	0
1374	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 11:40:21.404739	0
1375	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 11:41:21.51424	0
1376	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 11:42:21.644123	0
1377	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 11:43:21.755495	0
1378	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 11:44:21.889367	0
1379	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 11:45:21.994294	0
1380	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 11:46:22.086718	0
1381	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 11:47:22.200052	0
1382	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 11:48:22.434944	0
1383	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 11:49:22.574514	0
1384	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 11:50:22.667571	0
1385	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 11:51:22.797363	0
1386	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 11:52:22.881151	0
1387	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 11:53:22.980296	0
1388	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 11:54:23.069382	0
1389	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 11:55:23.137145	0
1390	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 11:56:23.31164	0
1391	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 11:57:23.413209	0
1392	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 11:58:23.571742	0
1393	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 11:59:23.711356	0
1394	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 12:00:23.785334	0
1395	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 12:01:23.840661	0
1396	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 12:02:23.880513	0
1397	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 12:03:23.976832	0
1398	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 12:04:24.01224	0
1399	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 13:00:57.09008	0
1400	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 13:01:57.186298	0
1401	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 13:02:57.270158	0
1402	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 13:03:57.335657	0
1403	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 13:04:57.409655	0
1404	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 13:05:57.518227	0
1405	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 13:06:57.642315	0
1406	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 13:07:57.804834	0
1407	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 13:08:57.898609	0
1408	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 13:09:57.952295	0
1409	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 13:10:58.067349	0
1410	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 13:11:58.186823	0
1411	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 13:12:58.280689	0
1412	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 13:13:58.374213	0
1413	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 13:14:58.485881	0
1414	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 13:15:58.5685	0
1415	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 13:16:58.643443	0
1416	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 13:17:58.732058	0
1417	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 13:18:58.860647	0
1418	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 13:19:58.963764	0
1419	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 13:20:59.027115	0
1420	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 13:21:59.124224	0
1421	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 13:22:59.215751	0
1422	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 13:23:59.359685	0
1423	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 13:24:59.672559	0
1424	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 13:25:59.787248	0
1425	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 13:26:59.891758	0
1426	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 13:27:59.944299	0
1427	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 13:29:00.11295	0
1428	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 13:30:00.19034	0
1429	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 13:31:00.30726	0
1430	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 13:32:00.44708	0
1431	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 13:33:00.543858	0
1432	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 13:34:00.695603	0
1433	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 13:35:00.912212	0
1434	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 13:36:00.953752	0
1435	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 13:37:01.280883	0
1436	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 13:38:01.439971	0
1437	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 13:39:01.686339	0
1438	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 13:40:01.872911	0
1439	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 13:41:02.0294	0
1440	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 13:42:02.199617	0
1441	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 13:43:02.515879	0
1442	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 13:44:02.637518	0
1443	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 13:45:02.879428	0
1444	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 13:46:03.002926	0
1445	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 13:47:03.237861	0
1446	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 13:48:03.493934	0
1447	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 13:49:03.558267	0
1448	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 13:50:03.629318	0
1449	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 13:51:03.773895	0
1450	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 13:52:03.949676	0
1451	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 13:53:04.16948	0
1452	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 13:54:04.256188	0
1453	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 13:55:04.446634	0
1454	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 13:56:04.679067	0
1455	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 13:57:04.978505	0
1456	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 13:58:05.052039	0
1457	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 13:59:05.268918	0
1458	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 14:00:05.367958	0
1459	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 14:01:05.566539	0
1460	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 14:02:05.68048	0
1461	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 14:03:05.83829	0
1462	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 14:04:06.005122	0
1463	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 14:05:06.129394	0
1464	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 14:06:06.303021	0
1465	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 14:07:06.465943	0
1466	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 14:08:06.689204	0
1467	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 14:09:06.836024	0
1468	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 14:10:06.975684	0
1469	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 14:11:07.106371	0
1470	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 14:12:07.295452	0
1471	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 14:13:07.364119	0
1472	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 14:14:07.462275	0
1473	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 14:15:07.548691	0
1474	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 14:16:07.64629	0
1475	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 14:17:07.743794	0
1476	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 14:18:07.834879	0
1477	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 14:19:07.924123	0
1478	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 14:20:08.027083	0
1479	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 14:21:08.119219	0
1480	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 14:22:08.201869	0
1481	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 14:23:08.275543	0
1482	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 14:24:08.340222	0
1483	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 14:25:08.427913	0
1484	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 14:26:08.540818	0
1485	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 14:27:08.677734	0
1486	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 14:28:08.825936	0
1487	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 14:29:08.930583	0
1488	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 14:30:09.019898	0
1489	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 14:31:09.2027	0
1490	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 14:32:09.292251	0
1491	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 14:33:09.420927	0
1492	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 14:34:09.572237	0
1493	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 14:35:09.637719	0
1494	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 14:36:09.693172	0
1495	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 14:37:09.766986	0
1496	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 14:38:09.865679	0
1497	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 14:39:09.968001	0
1498	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 14:40:10.127609	0
1499	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 14:41:10.193662	0
1500	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 14:42:10.346812	0
1501	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 14:43:10.440864	0
1502	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 14:44:10.512001	0
1503	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 14:45:10.629309	0
1504	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 14:46:10.751769	0
1505	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 14:47:10.957884	0
1506	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 14:48:11.061812	0
1507	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 14:49:11.139196	0
1508	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 14:50:11.281546	0
1509	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 14:51:11.391396	0
1510	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 14:52:11.594911	0
1511	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 14:53:11.693655	0
1512	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 14:54:11.764285	0
1513	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 14:55:11.930126	0
1514	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 14:56:12.03535	0
1515	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 14:57:12.116806	0
1516	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 14:58:12.426061	0
1517	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 14:59:12.528072	0
1518	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 15:00:12.685377	0
1519	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 15:01:12.772594	0
1520	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 15:02:12.849841	0
1521	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 15:03:12.940892	0
1522	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 15:04:13.059363	0
1523	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 15:05:13.195952	0
1524	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 15:06:13.405979	0
1525	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 15:07:13.503941	0
1526	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 15:08:13.659851	0
1527	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 15:09:13.743743	0
1528	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 15:10:13.851335	0
1529	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 15:11:13.965721	0
1530	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 15:12:14.103524	0
1531	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 15:13:14.183028	0
1532	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 15:14:14.280504	0
1533	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 15:15:14.379346	0
1534	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 15:16:14.492327	0
1535	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 15:17:14.643246	0
1536	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 15:18:14.917526	0
1537	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 15:19:15.048296	0
1538	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 15:20:15.163138	0
1539	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 15:21:15.237946	0
1540	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 15:22:15.259281	0
1541	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 15:23:15.285298	0
1542	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 15:29:15.275978	0
1543	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 15:30:15.343975	0
1544	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 15:31:15.427585	0
1545	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 15:32:15.505722	0
1546	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 15:33:15.684527	0
1547	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 15:34:15.751988	0
1548	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 15:35:15.851881	0
1549	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 15:36:16.012261	0
1550	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 15:37:16.108721	0
1551	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 15:38:16.421524	0
1552	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 15:39:16.52233	0
1553	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 15:40:16.623233	0
1554	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 15:41:16.693843	0
1555	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 15:42:16.836549	0
1556	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 15:43:16.921622	0
1557	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 15:44:17.003573	0
1558	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 15:45:17.081386	0
1559	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 15:46:17.214859	0
1560	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 15:47:17.307791	0
1561	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 15:48:17.383963	0
1562	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 15:49:17.474107	0
1563	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 15:50:17.547938	0
1564	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 15:51:17.625671	0
1565	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 15:52:17.71167	0
1566	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 15:53:17.796907	0
1567	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 15:54:17.878955	0
1568	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 15:55:17.961772	0
1569	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 15:56:18.041482	0
1570	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 15:57:18.100107	0
1571	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 15:58:18.187517	0
1572	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 15:59:18.274067	0
1573	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 16:00:18.40103	0
1574	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 16:01:18.488729	0
1575	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 16:02:18.576567	0
1576	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 16:03:18.707028	0
1577	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 16:04:18.784765	0
1578	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 16:05:18.864277	0
1579	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 16:06:18.947705	0
1580	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 16:07:19.046374	0
1581	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 16:08:19.147058	0
1582	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 16:09:19.249997	0
1583	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 16:10:19.353767	0
1584	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 16:11:19.453123	0
1585	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 16:12:19.543879	0
1586	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 16:13:19.615478	0
1587	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 16:14:19.681028	0
1588	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 16:15:19.755195	0
1589	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 16:16:19.832127	0
1590	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 16:17:19.913048	0
1591	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 16:18:19.997522	0
1592	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 16:19:20.108517	0
1593	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 16:20:20.201374	0
1594	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 16:21:20.338624	0
1595	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 16:22:20.432093	0
1596	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 16:23:20.54632	0
1597	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 16:24:20.58337	0
1598	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 16:25:20.644635	0
1599	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 16:26:20.760209	0
1600	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 16:27:20.892991	0
1601	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 16:28:20.976207	0
1602	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 16:29:21.059253	0
1603	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 16:30:21.128188	0
1604	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 16:31:21.207378	0
1605	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 16:32:21.301974	0
1606	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 16:33:21.404104	0
1607	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 16:34:21.486173	0
1608	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 16:35:21.566766	0
1609	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 16:36:21.648907	0
1610	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 16:37:21.722837	0
1611	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 16:38:21.854782	0
1612	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 16:39:21.92425	0
1613	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 16:40:22.047313	0
1614	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 16:41:22.112154	0
1615	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 16:42:22.192542	0
1616	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 16:43:22.293979	0
1617	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 16:44:22.371999	0
1618	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 16:45:22.518602	0
1619	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 16:46:22.601473	0
1620	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 16:47:22.675528	0
1621	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 16:48:22.770498	0
1622	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 16:49:22.845902	0
1623	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 16:50:22.903449	0
1624	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 16:51:22.990873	0
1625	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 16:52:23.06644	0
1626	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 16:53:23.165716	0
1627	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 16:54:23.235837	0
1628	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 16:55:23.325786	0
1629	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 16:56:23.414926	0
1630	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 16:57:23.495424	0
1631	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 16:58:23.578315	0
1632	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 16:59:23.679249	0
1633	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 17:00:23.761203	0
1634	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 17:01:23.862409	0
1635	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-16 17:02:23.973444	0
1636	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 08:51:08.398559	0
1637	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 08:52:10.750593	0
1638	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 08:53:10.851529	0
1639	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 08:54:10.928144	0
1640	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 08:55:10.998282	0
1641	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 08:56:11.137963	0
1642	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 08:57:11.191722	0
1643	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 08:58:11.267459	0
1644	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 08:59:11.470096	0
1645	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 09:00:11.556228	0
1646	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 09:01:11.652959	0
1647	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 09:02:11.75288	0
1648	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 09:03:11.824839	0
1649	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 09:04:11.963005	0
1650	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 09:05:12.024022	0
1651	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 09:06:12.227955	0
1652	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 09:07:12.319276	0
1653	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 09:08:12.487642	0
1654	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 09:09:12.589222	0
1655	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 09:10:12.67212	0
1656	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 09:11:12.742919	0
1657	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 09:12:12.805946	0
1658	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 09:13:12.900785	0
1659	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 09:14:12.997785	0
1660	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 09:15:13.100304	0
1661	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 09:16:13.167696	0
1662	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 09:17:13.25709	0
1663	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 09:18:13.318854	0
1664	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 09:19:13.408592	0
1665	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 09:20:13.514632	0
1666	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 09:21:13.576159	0
1667	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 09:22:13.647218	0
1668	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 09:23:13.733946	0
1669	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 09:24:13.79948	0
1670	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 09:25:13.908819	0
1671	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 09:26:13.980005	0
1672	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 09:27:14.070388	0
1673	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 09:28:14.179601	0
1674	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 09:29:14.207914	0
1675	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 09:30:14.260486	0
1676	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 09:31:14.379053	0
1677	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 09:32:14.427018	0
1678	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 09:33:14.535819	0
1679	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 09:34:14.618412	0
1680	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 09:35:14.725571	0
1681	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 09:36:14.783559	0
1682	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 09:37:14.881966	0
1683	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 09:38:14.958842	0
1684	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 09:39:15.054515	0
1685	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 09:40:15.124636	0
1686	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 09:41:15.222889	0
1687	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 09:42:15.310777	0
1688	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 09:43:15.362564	0
1689	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 09:44:15.487017	0
1690	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 09:45:15.56404	0
1691	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 09:46:15.6511	0
1692	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 09:47:15.742973	0
1693	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 09:48:15.858169	0
1694	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 09:49:15.943086	0
1695	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 09:50:16.028853	0
1696	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 09:51:16.122131	0
1697	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 09:52:16.182889	0
1698	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 09:53:16.235388	0
1699	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 09:54:16.313234	0
1700	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 09:55:16.399321	0
1701	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 09:56:16.450996	0
1702	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 09:57:16.497241	0
1703	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 09:58:16.542682	0
1704	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 09:59:16.633859	0
1705	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 10:00:16.724259	0
1706	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 10:01:16.818528	0
1707	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 10:02:16.887104	0
1708	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 10:03:16.977643	0
1709	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 10:04:17.084136	0
1710	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 10:05:17.158628	0
1711	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 10:06:17.237387	0
1712	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 10:07:17.303803	0
1713	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 10:08:17.444073	0
1714	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 10:09:17.521352	0
1715	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 10:10:17.553198	0
1716	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 10:11:17.595732	0
1717	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 10:12:17.670918	0
1718	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 10:13:17.723996	0
1719	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 10:14:17.796109	0
1720	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 10:15:17.837026	0
1721	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 10:16:17.933417	0
1722	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 10:17:17.985184	0
1723	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 10:18:18.073672	0
1724	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 10:19:18.198431	0
1725	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 10:20:18.276765	0
1726	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 10:21:18.357852	0
1727	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 10:22:18.429976	0
1728	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 10:23:18.503551	0
1729	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 10:24:18.590632	0
1730	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 10:25:18.685883	0
1731	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 10:26:18.775472	0
1732	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 10:27:18.832234	0
1733	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 10:28:18.940162	0
1734	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 10:29:18.984586	0
1735	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 10:30:19.040733	0
1736	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 10:31:19.120356	0
1737	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 10:32:19.211931	0
1738	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 10:33:19.25686	0
1739	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 10:34:19.362303	0
1740	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 10:35:19.509394	0
1741	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 10:36:19.584447	0
1742	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 10:37:19.654213	0
1743	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 10:38:19.769224	0
1744	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 10:39:19.870458	0
1745	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 10:40:19.944021	0
1746	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 10:41:20.012824	0
1747	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 10:42:20.110885	0
1748	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 10:43:20.181816	0
1749	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 10:44:20.241458	0
1750	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 10:45:20.340801	0
1751	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 10:46:20.421689	0
1752	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 10:47:20.51892	0
1753	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 10:48:20.599504	0
1754	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 10:49:20.729165	0
1755	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 10:50:20.812481	0
1756	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 10:51:20.844658	0
1757	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 10:52:20.930889	0
1758	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 10:53:21.030115	0
1759	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 10:54:21.14103	0
1760	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 10:55:21.214496	0
1761	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 10:56:21.280846	0
1762	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 10:57:21.360736	0
1763	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 10:58:21.464855	0
1764	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 10:59:21.524585	0
1765	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 11:00:21.569308	0
1766	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 11:01:21.654518	0
1767	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 11:02:21.788041	0
1768	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 11:03:21.862135	0
1769	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 11:04:21.963208	0
1770	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 11:05:22.061992	0
1771	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 11:06:22.121051	0
1772	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 11:07:22.178071	0
1773	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 11:08:22.196523	0
1774	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 11:09:22.220504	0
1775	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 11:10:22.293912	0
1776	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 11:11:42.144471	0
1777	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 11:12:42.270824	0
1778	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 11:13:42.385606	0
1779	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 11:14:42.454559	0
1780	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 11:15:42.529159	0
1781	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 11:16:42.580417	0
1782	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 11:17:42.655495	0
1783	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 11:18:42.742096	0
1784	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 11:19:42.830977	0
1785	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 11:20:42.911164	0
1786	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 11:21:43.00171	0
1787	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 11:22:43.110238	0
1788	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 11:23:43.178849	0
1789	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 11:24:43.302986	0
1790	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 11:25:43.396943	0
1791	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 11:26:43.484646	0
1792	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 11:27:43.605141	0
1793	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 11:28:43.694326	0
1794	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 11:29:43.781246	0
1795	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 11:30:43.862517	0
1796	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 11:31:43.945608	0
1797	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 11:32:44.019028	0
1798	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 11:33:44.099632	0
1799	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 11:34:44.203064	0
1800	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 11:35:44.286118	0
1801	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 11:36:44.355031	0
1802	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 11:37:44.484531	0
1803	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 11:38:44.586532	0
1804	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 11:39:44.712267	0
1805	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 11:40:44.813626	0
1806	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 11:41:44.948705	0
1807	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 11:42:45.056764	0
1808	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 11:43:45.144895	0
1809	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 11:44:45.225476	0
1810	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 11:45:45.32358	0
1811	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 11:46:45.420381	0
1812	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 11:47:45.553763	0
1813	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 11:48:45.593155	0
1814	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 11:49:45.681972	0
1815	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 11:50:45.897233	0
1816	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 11:51:45.981432	0
1817	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 11:52:46.055862	0
1818	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 11:53:46.135888	0
1819	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 11:54:46.197397	0
1820	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 11:55:46.309782	0
1821	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 11:56:46.382738	0
1822	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 11:57:46.447298	0
1823	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 11:58:46.51168	0
1824	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 11:59:46.583832	0
1825	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 12:00:46.704363	0
1826	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 12:01:46.76714	0
1827	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 12:02:46.82892	0
1828	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 12:03:46.933449	0
1829	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 12:04:46.98655	0
1830	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 12:05:47.067058	0
1831	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 12:06:47.152932	0
1832	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 12:07:47.255291	0
1833	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 12:08:47.307897	0
1834	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 12:09:47.379072	0
1835	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 12:10:47.453268	0
1836	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 12:11:47.540776	0
1837	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 12:12:47.621609	0
1838	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 12:13:47.697985	0
1839	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 12:14:47.812461	0
1840	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 12:15:47.914184	0
1841	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 12:16:47.973459	0
1842	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 12:17:48.032437	0
1843	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 12:18:48.091869	0
1844	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 12:19:48.119628	0
1845	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 12:20:48.147481	0
1846	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 12:21:48.171398	0
1847	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 12:59:23.082079	0
1848	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 13:00:23.150848	0
1849	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 13:01:23.227293	0
1850	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 13:02:23.300739	0
1851	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 13:03:23.378258	0
1852	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 13:04:23.459297	0
1853	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 13:05:23.543954	0
1854	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 13:06:23.61667	0
1855	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 13:07:23.744218	0
1856	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 13:08:23.819932	0
1857	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 13:09:23.929796	0
1858	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 13:10:23.985382	0
1859	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 13:11:24.077349	0
1860	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 13:12:24.195616	0
1861	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 13:13:24.281129	0
1862	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 13:14:24.41704	0
1863	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 13:15:24.501789	0
1864	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 13:16:24.55766	0
1865	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 13:17:24.630705	0
1866	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 13:18:24.729738	0
1867	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 13:19:24.828501	0
1868	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 13:20:24.928864	0
1869	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 13:21:25.00137	0
1870	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 13:22:25.045221	0
1871	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 13:23:25.115914	0
1872	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 13:24:25.19357	0
1873	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 13:25:25.250048	0
1874	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 13:26:25.289411	0
1875	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 13:27:25.387722	0
1876	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 13:28:25.478153	0
1877	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 13:29:25.58686	0
1878	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 13:30:25.657695	0
1879	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 13:31:25.73734	0
1880	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 13:32:25.824421	0
1881	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 13:33:25.899547	0
1882	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 13:34:25.942442	0
1883	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 13:35:26.036099	0
1884	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 13:36:26.16569	0
1885	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 13:37:26.232965	0
1886	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 13:38:26.340554	0
1887	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 13:39:26.443101	0
1888	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 13:40:26.543509	0
1889	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 13:41:26.631808	0
1890	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 13:42:26.747894	0
1891	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 13:43:26.856166	0
1892	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 13:44:26.92535	0
1893	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 13:45:27.002917	0
1894	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 13:46:27.101244	0
1895	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 13:47:27.220328	0
1896	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 13:48:27.322045	0
1897	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 13:49:27.385837	0
1898	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 13:50:27.49924	0
1899	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 13:51:27.606821	0
1900	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 13:52:27.706122	0
1901	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 13:53:27.834813	0
1902	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 13:54:27.944666	0
1903	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 13:55:28.003481	0
1904	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 13:56:28.100081	0
1905	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 13:57:28.17566	0
1906	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 13:58:28.289705	0
1907	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 13:59:28.356999	0
1908	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 14:00:28.455355	0
1909	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 14:01:28.582437	0
1910	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 14:02:28.659642	0
1911	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 14:03:28.700854	0
1912	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 14:04:28.801347	0
1913	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 14:05:28.847988	0
1914	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 14:06:28.935479	0
1915	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 14:07:29.005521	0
1916	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 14:08:29.120817	0
1917	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 14:09:29.175484	0
1918	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 14:10:29.264455	0
1919	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 14:11:29.376745	0
1920	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 14:12:29.468709	0
1921	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 14:13:29.5377	0
1922	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 14:14:29.649843	0
1923	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 14:15:29.711047	0
1924	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 14:16:29.784249	0
1925	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 14:17:29.868608	0
1926	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 14:18:29.974216	0
1927	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 14:19:30.055362	0
1928	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 14:20:30.121514	0
1929	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 14:21:30.229451	0
1930	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 14:22:30.342181	0
1931	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 14:23:30.432734	0
1932	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 14:24:30.512616	0
1933	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 14:25:30.592702	0
1934	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 14:26:30.661732	0
1935	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 14:27:30.773499	0
1936	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 14:28:30.823015	0
1937	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 14:29:30.887348	0
1938	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 14:30:31.002164	0
1939	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 14:31:31.077001	0
1940	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 14:32:31.166235	0
1941	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 14:33:31.259645	0
1942	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 14:34:31.366688	0
1943	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 14:35:31.401575	0
1944	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 14:36:31.439247	0
1945	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 14:37:31.477253	0
1946	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 14:38:31.49257	0
1947	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 14:39:31.567681	0
1948	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 14:40:31.633193	0
1949	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 14:41:31.708768	0
1950	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 14:42:31.776157	0
1951	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 14:43:31.862088	0
1952	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 14:44:31.939053	0
1953	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 14:45:32.024919	0
1954	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 14:46:32.119066	0
1955	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 14:47:32.229621	0
1956	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 14:48:32.329133	0
1957	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 14:49:32.406003	0
1958	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 14:50:32.539411	0
1959	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 14:51:32.59232	0
1960	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 14:52:32.726337	0
1961	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 14:53:32.809187	0
1962	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 14:54:32.878231	0
1963	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 14:55:32.974455	0
1964	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 14:56:33.08527	0
1965	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 14:57:33.177817	0
1966	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 14:58:33.263762	0
1967	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 14:59:33.372915	0
1968	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 15:00:33.443102	0
1969	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 15:01:33.515316	0
1970	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 15:02:33.59383	0
1971	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 15:03:33.679937	0
1972	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 15:04:33.803174	0
1973	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 15:05:33.896223	0
1974	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 15:06:33.96733	0
1975	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 15:07:34.06022	0
1976	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 15:08:34.132603	0
1977	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 15:09:34.223415	0
1978	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 15:10:34.305989	0
1979	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 15:11:34.436453	0
1980	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 15:12:34.478988	0
1981	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 15:13:34.600026	0
1982	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 15:14:34.721964	0
1983	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 15:15:34.795406	0
1984	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 15:16:34.891244	0
1985	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 15:17:35.00291	0
1986	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 15:18:35.098885	0
1987	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 15:19:35.215338	0
1988	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 15:20:35.305144	0
1989	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 15:21:35.403017	0
1990	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 15:22:35.474686	0
1991	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 15:23:35.599726	0
1992	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 15:24:35.679055	0
1993	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 15:25:35.747369	0
1994	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 15:26:35.819116	0
1995	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 15:27:35.920947	0
1996	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 15:28:35.999989	0
1997	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 15:29:36.098073	0
1998	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 15:30:36.171889	0
1999	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 15:31:36.268629	0
2000	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 15:32:36.362918	0
2001	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 15:33:36.485816	0
2002	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 15:34:36.566831	0
2003	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 15:35:36.675213	0
2004	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 15:36:36.728155	0
2005	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 15:37:36.829755	0
2006	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 15:38:36.961317	0
2007	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 15:39:37.031883	0
2008	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 15:40:37.118669	0
2009	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 15:41:37.245168	0
2010	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 15:42:37.394131	0
2011	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 15:43:37.407797	0
2012	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 15:44:37.429334	0
2013	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 15:45:37.476982	0
2014	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 15:46:37.51998	0
2015	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 15:47:37.555729	0
2016	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 16:04:54.309509	0
2017	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 16:05:54.385615	0
2018	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 16:06:54.460422	0
2019	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 16:07:54.552479	0
2020	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 16:08:54.607593	0
2021	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 16:09:54.691434	0
2022	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 16:10:54.748677	0
2023	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 16:11:54.85552	0
2024	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 16:12:54.981651	0
2025	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 16:13:55.05149	0
2026	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 16:14:55.135844	0
2027	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 16:15:55.247043	0
2028	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 16:16:55.322944	0
2029	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 16:17:55.383687	0
2030	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 16:18:55.470086	0
2031	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 16:19:55.553087	0
2032	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 16:20:55.643564	0
2033	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 16:21:55.747934	0
2034	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 16:22:55.868145	0
2035	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 16:23:55.938765	0
2036	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 16:24:56.040668	0
2037	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 16:25:56.073995	0
2038	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 16:26:56.199461	0
2039	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 16:27:56.272106	0
2040	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 16:28:56.353854	0
2041	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 16:29:56.435547	0
2042	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 16:30:56.537471	0
2043	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 16:31:56.612816	0
2044	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 16:32:56.668967	0
2045	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 16:33:56.743374	0
2046	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 16:34:56.885994	0
2047	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 16:35:56.970323	0
2048	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 16:36:57.054901	0
2049	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 16:37:57.112519	0
2050	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 16:38:57.21271	0
2051	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 16:39:57.307884	0
2052	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 16:40:57.468496	0
2053	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 16:41:57.59994	0
2054	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 16:42:57.693512	0
2055	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 16:43:57.782376	0
2056	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 16:44:57.838979	0
2057	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 16:45:57.90952	0
2058	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 16:46:57.993575	0
2059	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 16:47:58.087835	0
2060	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 16:48:58.147711	0
2061	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 16:49:58.235791	0
2062	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 16:50:58.294979	0
2063	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 16:51:58.383467	0
2064	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 16:52:58.466701	0
2065	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 16:53:58.53335	0
2066	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 16:54:58.599019	0
2067	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 16:55:58.745742	0
2068	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 16:56:58.896952	0
2069	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 16:57:58.972323	0
2070	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 16:58:59.060341	0
2071	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 16:59:59.155453	0
2072	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 17:00:59.261886	0
2073	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 17:01:59.387817	0
2074	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 17:02:59.489682	0
2075	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 17:03:59.603399	0
2076	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 17:04:59.714436	0
2077	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 17:05:59.831523	0
2078	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 17:06:59.929678	0
2079	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 17:08:00.023156	0
2080	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 17:09:00.130312	0
2081	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 17:10:00.219954	0
2082	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 17:11:00.275804	0
2083	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 17:12:00.364846	0
2084	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 17:13:00.430023	0
2085	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 17:14:00.541034	0
2086	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 17:15:00.632147	0
2087	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 17:16:00.727066	0
2088	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 17:17:00.836666	0
2089	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 17:18:00.912857	0
2090	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 17:19:00.986941	0
2091	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 17:20:01.062888	0
2092	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 17:21:01.138666	0
2093	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 17:22:01.216855	0
2094	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 17:23:01.301996	0
2095	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 17:24:01.397837	0
2096	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 17:25:01.505192	0
2097	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 17:26:01.580494	0
2098	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 17:27:01.645077	0
2099	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 17:28:01.758981	0
2100	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 17:29:01.83233	0
2101	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 17:30:01.941115	0
2102	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 17:31:02.008537	0
2103	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 17:32:02.120775	0
2104	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 17:33:02.195857	0
2105	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 17:34:02.271122	0
2106	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 17:35:02.316762	0
2107	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 17:36:02.371577	0
2108	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 17:37:02.459083	0
2109	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 17:38:02.520608	0
2110	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 17:39:02.591498	0
2111	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 17:40:02.677399	0
2112	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 17:41:02.747901	0
2113	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 17:42:02.83811	0
2114	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 17:43:02.912097	0
2115	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 17:44:03.032828	0
2116	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 17:45:03.11607	0
2117	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 17:46:03.223389	0
2118	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 17:47:03.300577	0
2119	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 17:48:03.386804	0
2120	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 17:49:03.462252	0
2121	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 17:50:03.550323	0
2122	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 17:51:03.627206	0
2123	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 17:52:03.71183	0
2124	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 17:53:03.81838	0
2125	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 17:54:03.906602	0
2126	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 17:55:03.986017	0
2127	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 17:56:04.056966	0
2128	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 17:57:04.158936	0
2129	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 17:58:04.238979	0
2130	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 17:59:04.300263	0
2131	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 18:00:04.391845	0
2132	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 18:01:04.480044	0
2133	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-19 18:02:04.546755	0
2134	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 08:51:25.699249	0
2135	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 08:52:25.775979	0
2136	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 08:53:25.860579	0
2137	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 08:54:25.927332	0
2138	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 08:55:26.034889	0
2139	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 08:56:26.154168	0
2140	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 08:57:26.226995	0
2141	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 08:58:26.312201	0
2142	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 08:59:26.399772	0
2143	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 09:00:26.505748	0
2144	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 09:01:26.58687	0
2145	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 09:02:26.700177	0
2146	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 09:03:26.791311	0
2147	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 09:04:26.890766	0
2148	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 09:05:26.967429	0
2149	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 09:06:27.076263	0
2150	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 09:07:27.184379	0
2151	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 09:08:27.300149	0
2152	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 09:09:27.369079	0
2153	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 09:10:27.451717	0
2154	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 09:11:27.523791	0
2155	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 09:12:27.604792	0
2156	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 09:13:27.656596	0
2157	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 09:14:27.711647	0
2158	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 09:15:27.794391	0
2159	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 09:16:27.880812	0
2160	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 09:17:27.948162	0
2161	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 09:18:28.002369	0
2162	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 09:19:28.067953	0
2163	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 09:20:28.170323	0
2164	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 09:21:28.28345	0
2165	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 09:22:28.355375	0
2166	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 09:23:28.451956	0
2167	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 09:24:28.537731	0
2168	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 09:25:28.615246	0
2169	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 09:26:28.684348	0
2170	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 09:27:28.787326	0
2171	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 09:28:28.876093	0
2172	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 09:29:28.939845	0
2173	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 09:30:29.016432	0
2174	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 09:31:29.119274	0
2175	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 09:32:29.199242	0
2176	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 09:33:29.276263	0
2177	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 09:34:29.353519	0
2178	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 09:35:29.43984	0
2179	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 09:36:29.521594	0
2180	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 09:37:29.605867	0
2181	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 09:38:29.673099	0
2182	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 09:39:29.732264	0
2183	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 09:40:29.816814	0
2184	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 09:41:29.917643	0
2185	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 09:42:29.991762	0
2186	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 09:43:30.073867	0
2187	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 09:44:30.143234	0
2188	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 09:45:30.206167	0
2189	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 09:46:30.283044	0
2190	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 09:47:30.354614	0
2191	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 09:48:30.41585	0
2192	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 09:49:30.498146	0
2193	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 09:50:30.599621	0
2194	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 09:51:30.741771	0
2195	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 09:52:30.81078	0
2196	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 09:53:30.888355	0
2197	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 09:54:30.973168	0
2198	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 09:55:31.058729	0
2199	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 09:56:31.137781	0
2200	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 09:57:31.251246	0
2201	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 09:58:31.336078	0
2202	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 09:59:31.474806	0
2203	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 10:00:31.577838	0
2204	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 10:01:31.656424	0
2205	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 10:02:31.728422	0
2206	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 10:03:31.802366	0
2207	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 10:04:31.912197	0
2208	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 10:05:31.979181	0
2209	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 10:06:32.047617	0
2210	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 10:07:32.104604	0
2211	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 10:08:32.198224	0
2212	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 10:09:32.345077	0
2213	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 10:10:32.451213	0
2214	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 10:11:32.539078	0
2215	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 10:12:32.616285	0
2216	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 10:13:32.692952	0
2217	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 10:14:32.778793	0
2218	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 10:15:32.890812	0
2219	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 10:16:32.96498	0
2220	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 10:17:33.037201	0
2221	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 10:18:33.145618	0
2222	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 10:19:33.260792	0
2223	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 10:20:33.37742	0
2224	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 10:21:33.465958	0
2225	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 10:22:33.556447	0
2226	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 10:23:33.618911	0
2227	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 10:24:33.763682	0
2228	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 10:25:33.843745	0
2229	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 10:26:33.943246	0
2230	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 10:27:34.038248	0
2231	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 10:28:34.1257	0
2232	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 10:29:34.197974	0
2233	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 10:30:34.281814	0
2234	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 10:31:34.386457	0
2235	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 10:32:34.477203	0
2236	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 10:33:34.582426	0
2237	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 10:34:34.655154	0
2238	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 10:35:34.760473	0
2239	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 10:36:34.849488	0
2240	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 10:37:34.93701	0
2241	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 10:38:35.011081	0
2242	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 10:39:35.127618	0
2243	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 10:40:35.219841	0
2244	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 10:41:35.306889	0
2245	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 10:42:35.404163	0
2246	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 10:43:35.508689	0
2247	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 10:44:35.587912	0
2248	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 10:45:35.698651	0
2249	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 10:46:35.820345	0
2250	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 10:47:35.897349	0
2251	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 10:48:35.960752	0
2252	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 10:49:36.045189	0
2253	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 10:50:36.125549	0
2254	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 10:51:36.222972	0
2255	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 10:52:36.277635	0
2256	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 10:53:36.347478	0
2257	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 10:54:36.430004	0
2258	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 10:55:36.532218	0
2259	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 10:56:36.640901	0
2260	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 10:57:36.728919	0
2261	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 10:58:36.854346	0
2262	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 10:59:36.925039	0
2263	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 11:00:37.010924	0
2264	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 11:01:37.075971	0
2265	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 11:02:37.17112	0
2266	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 11:03:37.245961	0
2267	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 11:04:37.309599	0
2268	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 11:05:37.360616	0
2269	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 11:06:37.440749	0
2270	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 11:07:37.538036	0
2271	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 11:08:37.634304	0
2272	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 11:09:37.715423	0
2273	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 11:10:37.781206	0
2274	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 11:11:37.8382	0
2275	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 11:12:37.909195	0
2276	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 11:13:38.004058	0
2277	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 11:14:38.059104	0
2278	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 11:15:38.138951	0
2279	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 11:16:38.205647	0
2280	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 11:17:38.277372	0
2281	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 11:18:38.367296	0
2282	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 11:19:38.490998	0
2283	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 11:20:38.578017	0
2284	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 11:21:38.676741	0
2285	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 11:22:38.763288	0
2286	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 11:23:38.850405	0
2287	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 11:24:38.968942	0
2288	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 11:25:39.110417	0
2289	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 11:26:39.229661	0
2290	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 11:27:39.296514	0
2291	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 11:28:39.382753	0
2292	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 11:29:39.449642	0
2293	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 11:30:39.557782	0
2294	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 11:31:39.658905	0
2295	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 11:32:39.736901	0
2296	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 11:33:39.812389	0
2297	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 11:34:39.91553	0
2298	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 11:35:39.982365	0
2299	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 11:36:40.073043	0
2300	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 11:37:40.227318	0
2301	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 11:38:40.304961	0
2302	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 11:39:40.385316	0
2303	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 11:40:40.471626	0
2304	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 11:41:40.558938	0
2305	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 11:42:40.632415	0
2306	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 11:43:40.710837	0
2307	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 11:44:40.795926	0
2308	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 11:45:40.913224	0
2309	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 11:46:41.056632	0
2310	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 11:47:41.096288	0
2311	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 11:48:41.131051	0
2312	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 11:49:41.204471	0
2313	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 11:50:41.251116	0
2314	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 11:51:41.311755	0
2315	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 11:57:21.676502	0
2316	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 11:58:21.78156	0
2317	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 11:59:21.865777	0
2318	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 12:00:21.950442	0
2319	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 12:01:22.103657	0
2320	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 12:02:22.176903	0
2321	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 12:03:22.25679	0
2322	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 12:04:22.337473	0
2323	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 12:05:22.415731	0
2324	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 12:06:22.559739	0
2325	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 12:07:22.714346	0
2326	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 12:08:22.807898	0
2327	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 12:09:22.936318	0
2328	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 12:10:23.017163	0
2329	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 12:11:23.121828	0
2330	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 12:12:23.215938	0
2331	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 12:13:23.318446	0
2332	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 12:14:23.442818	0
2333	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 12:15:23.588121	0
2334	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 12:16:23.670325	0
2335	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 12:17:23.735071	0
2336	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 12:18:23.836236	0
2337	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 12:19:23.921363	0
2338	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 12:20:23.982594	0
2339	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 12:21:24.055002	0
2340	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 12:22:24.099369	0
2341	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 12:23:24.161368	0
2342	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 12:24:24.199624	0
2343	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 13:00:07.171074	0
2344	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 13:01:07.229702	0
2345	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 13:02:07.327308	0
2346	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 13:03:07.41814	0
2347	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 13:04:07.51351	0
2348	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 13:05:07.617133	0
2349	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 13:06:07.743729	0
2350	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 13:07:07.820853	0
2351	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 13:08:07.914643	0
2352	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 13:09:07.986965	0
2353	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 13:10:08.088523	0
2354	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 13:11:08.191985	0
2355	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 13:12:08.280594	0
2356	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 13:13:08.382465	0
2357	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 13:14:08.485935	0
2358	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 13:15:08.573806	0
2359	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 13:16:08.652357	0
2360	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 13:17:08.727933	0
2361	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 13:18:08.806812	0
2362	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 13:19:08.882709	0
2363	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 13:20:09.013332	0
2364	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 13:21:09.141187	0
2365	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 13:22:09.221831	0
2366	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 13:23:09.300492	0
2367	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 13:24:09.36152	0
2368	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 13:25:09.401721	0
2369	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 13:26:09.568162	0
2370	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 13:27:09.634143	0
2371	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 13:28:09.705142	0
2372	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 13:29:09.774093	0
2373	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 13:30:09.906808	0
2374	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 13:31:09.980252	0
2375	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 13:32:10.041784	0
2376	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 13:33:10.117647	0
2377	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 13:34:10.175779	0
2378	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 13:35:10.253661	0
2379	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 13:36:10.329593	0
2380	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 13:37:10.435303	0
2381	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 13:38:10.510251	0
2382	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 13:39:10.575347	0
2383	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 13:40:10.677173	0
2384	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 13:41:10.752303	0
2385	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 13:42:10.843082	0
2386	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 13:43:10.909796	0
2387	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 13:44:10.974548	0
2388	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 13:45:11.078044	0
2389	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 13:46:11.176551	0
2390	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 13:47:11.248517	0
2391	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 13:48:11.339892	0
2392	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 13:49:11.426994	0
2393	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 13:50:11.518019	0
2394	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 13:51:11.602679	0
2395	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 13:52:11.691209	0
2396	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 13:53:11.79438	0
2397	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 13:54:11.874743	0
2398	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 13:55:11.961827	0
2399	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 13:56:12.0704	0
2400	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 13:57:12.161315	0
2401	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 13:58:12.25136	0
2402	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 13:59:12.31997	0
2403	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 14:00:12.398232	0
2404	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 14:01:12.466835	0
2405	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 14:02:12.567702	0
2406	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 14:03:12.644375	0
2407	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 14:04:12.726144	0
2408	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 14:05:12.814961	0
2409	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 14:06:12.901172	0
2410	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 14:07:12.991001	0
2411	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 14:08:13.056775	0
2412	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 14:09:13.158058	0
2413	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 14:10:13.234502	0
2414	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 14:11:13.323612	0
2415	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 14:12:13.406045	0
2416	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 14:13:13.518155	0
2417	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 14:14:13.563586	0
2418	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 14:15:13.636357	0
2419	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 14:16:13.705163	0
2420	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 14:17:13.819949	0
2421	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 14:18:13.899119	0
2422	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 14:19:14.00322	0
2423	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 14:20:14.053031	0
2424	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 14:21:14.13145	0
2425	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 14:22:14.247284	0
2426	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 14:23:14.391967	0
2427	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 14:24:14.433729	0
2428	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 14:25:14.53796	0
2429	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 14:26:14.635832	0
2430	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 14:27:14.691003	0
2431	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 14:28:14.779841	0
2432	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 14:29:14.858171	0
2433	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 14:30:14.925836	0
2434	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 14:31:14.989581	0
2435	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 14:32:15.064473	0
2436	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 14:33:15.172766	0
2437	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 14:34:15.215656	0
2438	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 14:35:15.325569	0
2439	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 14:36:15.450734	0
2440	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 14:37:15.510191	0
2441	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 14:38:15.591477	0
2442	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 14:39:15.646291	0
2443	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 14:40:15.723598	0
2444	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 14:41:15.786436	0
2445	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 14:42:15.870603	0
2446	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 14:43:15.94607	0
2447	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 14:44:16.044257	0
2448	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 14:45:16.129874	0
2449	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 14:46:16.260436	0
2450	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 14:47:16.341242	0
2451	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 14:48:16.405245	0
2452	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 14:49:16.50623	0
2453	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 14:50:16.597374	0
2454	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 14:51:16.715563	0
2455	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 14:52:16.781846	0
2456	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 14:53:16.853793	0
2457	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 14:54:16.959955	0
2458	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 14:55:17.058735	0
2459	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 14:56:17.147403	0
2460	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 14:57:17.220608	0
2461	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 14:58:17.261101	0
2462	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 14:59:17.333843	0
2463	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 15:00:17.393087	0
2464	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 15:01:17.47904	0
2465	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 15:02:17.633299	0
2466	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 15:03:17.733098	0
2467	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 15:04:17.792848	0
2468	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 15:05:17.849634	0
2469	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 15:06:17.927772	0
2470	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 15:07:18.008178	0
2471	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 15:08:18.107402	0
2472	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 15:09:18.194928	0
2473	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 15:10:18.258964	0
2474	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 15:11:18.351944	0
2475	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 15:12:18.422348	0
2476	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 15:13:18.49683	0
2477	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 15:14:18.55478	0
2478	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 15:15:18.649761	0
2479	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 15:16:18.751756	0
2480	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 15:17:18.892136	0
2481	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 15:18:18.975965	0
2482	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 15:19:19.072114	0
2483	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 15:20:19.147199	0
2484	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 15:21:19.235226	0
2485	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 15:22:19.328897	0
2486	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 15:23:19.421885	0
2487	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 15:24:19.518634	0
2488	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 15:25:19.611431	0
2489	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 15:26:19.682575	0
2490	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 15:27:19.755981	0
2491	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 15:28:19.881385	0
2492	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 15:29:19.955603	0
2493	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 15:30:20.087216	0
2494	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 15:31:20.181861	0
2495	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 15:32:20.272939	0
2496	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 15:33:20.371118	0
2497	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 15:34:20.448609	0
2498	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 15:35:20.591829	0
2499	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 15:36:20.698181	0
2500	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 15:37:20.813021	0
2501	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 15:38:20.889956	0
2502	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 15:39:20.993411	0
2503	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 15:40:21.088063	0
2504	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 15:41:21.167356	0
2505	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 15:42:21.327445	0
2506	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 15:43:21.444318	0
2507	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 15:44:21.563879	0
2508	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 15:45:21.658626	0
2509	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 15:46:21.75613	0
2510	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 15:47:21.839271	0
2511	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 15:48:21.938958	0
2512	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 15:49:22.098363	0
2513	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 15:50:22.196606	0
2514	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 15:51:22.284501	0
2515	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 15:52:22.355721	0
2516	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 15:53:22.453361	0
2517	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 15:54:22.567594	0
2518	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 15:55:22.696427	0
2519	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 15:56:22.785064	0
2520	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 15:57:22.920777	0
2521	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 15:58:23.011505	0
2522	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 15:59:23.100509	0
2523	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 16:00:23.171339	0
2524	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 16:01:23.279609	0
2525	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 16:02:23.368643	0
2526	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 16:03:23.486969	0
2527	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 16:04:23.55862	0
2528	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 16:05:23.606935	0
2529	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 16:06:23.679693	0
2530	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 16:07:23.79525	0
2531	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 16:08:23.857956	0
2532	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 16:09:23.937872	0
2533	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 16:10:23.996376	0
2534	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 16:11:24.135872	0
2535	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 16:12:24.237416	0
2536	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 16:13:24.301163	0
2537	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 16:14:24.402563	0
2538	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 16:15:24.482068	0
2539	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 16:16:24.554662	0
2540	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 16:17:24.661225	0
2541	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 16:18:24.732536	0
2542	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 16:19:24.811422	0
2543	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 16:20:24.903964	0
2544	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 16:21:24.982554	0
2545	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 16:22:25.059818	0
2546	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 16:23:25.19247	0
2547	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 16:24:25.321105	0
2548	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 16:25:25.402552	0
2549	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 16:26:25.486854	0
2550	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 16:27:25.578018	0
2551	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 16:28:25.652893	0
2552	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 16:29:25.756268	0
2553	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 16:30:25.815367	0
2554	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 16:31:25.921273	0
2555	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 16:32:25.984275	0
2556	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 16:33:26.067374	0
2557	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 16:34:26.137015	0
2558	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 16:35:26.243874	0
2559	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 16:36:26.31159	0
2560	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 16:42:18.243575	0
2561	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 16:43:18.326979	0
2562	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 16:44:18.435899	0
2563	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 16:45:18.528625	0
2564	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 16:46:18.61592	0
2565	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 16:47:18.697955	0
2566	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 16:48:18.822045	0
2567	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 16:49:18.914391	0
2568	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 16:50:18.99265	0
2569	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 16:51:19.132988	0
2570	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 16:52:19.234162	0
2571	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 16:53:19.298424	0
2572	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 16:54:19.418313	0
2573	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 16:55:19.494715	0
2574	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 16:56:19.584646	0
2575	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 16:57:19.662147	0
2576	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 16:58:19.731856	0
2577	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 16:59:19.816136	0
2578	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 17:00:19.904625	0
2579	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 17:01:19.990345	0
2580	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 17:02:20.106286	0
2581	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 17:03:20.202547	0
2582	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 17:04:20.307085	0
2583	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 17:05:20.386801	0
2584	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 17:06:20.483857	0
2585	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 17:07:20.573015	0
2586	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 17:08:20.656127	0
2587	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 17:09:20.748044	0
2588	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 17:10:20.823337	0
2589	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 17:11:20.916077	0
2590	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 17:12:20.977959	0
2591	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 17:13:21.063743	0
2592	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 17:14:21.191716	0
2593	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 17:15:21.296642	0
2594	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 17:16:21.424437	0
2595	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 17:17:21.531763	0
2596	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 17:18:21.614211	0
2597	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 17:19:21.701336	0
2598	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 17:20:21.824502	0
2599	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 17:21:21.912162	0
2600	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 17:22:21.996645	0
2601	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 17:23:22.117421	0
2602	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 17:24:22.218304	0
2603	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 17:25:22.326585	0
2604	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 17:26:22.435488	0
2605	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 17:27:22.568158	0
2606	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 17:28:22.719564	0
2607	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 17:29:22.793932	0
2608	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 17:30:22.879917	0
2609	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 17:31:22.979376	0
2610	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 17:32:23.14315	0
2611	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 17:33:23.23124	0
2612	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 17:34:23.408092	0
2613	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 17:35:23.484702	0
2614	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 17:36:23.563671	0
2615	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 17:37:23.6405	0
2616	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 17:38:23.718065	0
2617	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 17:39:23.801735	0
2618	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 17:40:23.951498	0
2619	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 17:41:24.005241	0
2620	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 17:42:24.087068	0
2621	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 17:43:24.173847	0
2622	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 17:44:24.265226	0
2623	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 17:45:24.341219	0
2624	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 17:46:24.418814	0
2625	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 17:47:24.50057	0
2626	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 17:48:24.609969	0
2627	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 17:49:24.687887	0
2628	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 17:50:24.77251	0
2629	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 17:51:24.886306	0
2630	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 17:52:24.972522	0
2631	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 17:53:25.048753	0
2632	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 17:54:25.133117	0
2633	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 17:55:25.217572	0
2634	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 17:56:25.30674	0
2635	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 17:57:25.372602	0
2636	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 17:58:25.419887	0
2637	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 17:59:25.530791	0
2638	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-20 18:00:25.609039	0
2639	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 08:52:51.556337	0
2640	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 08:53:51.680708	0
2641	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 08:54:51.833536	0
2642	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 08:55:51.930905	0
2643	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 08:56:52.038896	0
2644	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 08:57:52.202319	0
2645	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 08:58:52.275967	0
2646	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 08:59:52.360159	0
2647	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 09:00:52.450058	0
2648	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 09:01:52.5083	0
2649	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 09:02:52.601838	0
2650	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 09:03:52.682607	0
2651	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 09:04:52.759281	0
2652	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 09:05:52.851862	0
2653	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 09:06:52.953151	0
2654	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 09:07:52.980677	0
2655	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 09:08:53.06496	0
2656	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 09:09:53.15893	0
2657	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 09:10:53.235996	0
2658	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 09:11:53.301375	0
2659	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 09:12:53.424018	0
2660	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 09:13:53.468407	0
2661	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 09:14:53.586516	0
2662	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 09:15:53.652632	0
2663	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 09:16:53.764474	0
2664	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 09:17:53.823842	0
2665	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 09:18:53.917852	0
2666	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 09:19:53.989592	0
2667	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 09:20:54.068706	0
2668	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 09:21:54.156936	0
2669	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 09:22:54.257271	0
2670	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 09:23:54.346423	0
2671	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 09:24:54.440998	0
2672	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 09:25:54.528189	0
2673	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 09:26:54.627029	0
2674	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 09:27:54.697229	0
2675	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 09:28:54.798448	0
2676	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 09:29:54.912692	0
2677	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 09:30:55.011081	0
2678	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 09:31:55.082093	0
2679	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 09:32:55.147354	0
2680	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 09:33:55.223253	0
2681	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 09:34:55.316214	0
2682	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 09:35:55.413439	0
2683	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 09:36:55.507553	0
2684	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 09:37:55.595253	0
2685	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 09:38:55.676097	0
2686	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 09:39:55.752475	0
2687	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 09:40:55.849077	0
2688	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 09:41:55.936529	0
2689	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 09:42:56.049312	0
2690	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 09:43:56.167551	0
2691	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 09:44:56.228713	0
2692	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 09:45:56.315828	0
2693	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 09:46:56.426991	0
2694	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 09:47:56.535156	0
2695	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 09:48:56.582629	0
2696	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 09:49:56.666482	0
2697	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 09:50:56.772535	0
2698	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 09:51:56.899276	0
2699	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 09:52:56.967686	0
2700	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 09:53:57.10288	0
2701	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 09:54:57.205195	0
2702	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 09:55:57.285725	0
2703	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 09:56:57.351018	0
2704	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 09:57:57.437189	0
2705	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 09:58:57.51861	0
2706	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 09:59:57.598546	0
2707	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 10:00:57.69343	0
2708	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 10:01:57.73161	0
2709	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 10:02:57.810015	0
2710	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 10:03:57.861599	0
2711	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 10:04:57.952969	0
2712	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 10:05:58.004904	0
2713	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 10:06:58.077225	0
2714	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 10:07:58.168794	0
2715	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 10:08:58.199045	0
2716	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 10:09:58.271295	0
2717	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 10:10:58.347412	0
2718	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 10:11:58.444946	0
2719	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 10:12:58.533496	0
2720	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 10:13:58.630214	0
2721	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 10:14:58.745112	0
2722	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 10:15:58.868724	0
2723	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 10:16:58.967953	0
2724	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 10:17:59.044304	0
2725	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 10:18:59.165656	0
2726	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 10:19:59.219127	0
2727	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 10:20:59.337917	0
2728	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 10:21:59.393016	0
2729	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 10:22:59.444336	0
2730	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 10:23:59.509617	0
2731	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 10:24:59.603711	0
2732	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 10:25:59.704736	0
2733	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 10:26:59.804474	0
2734	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 10:28:00.126976	0
2735	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 10:29:00.229789	0
2736	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 10:30:00.319168	0
2737	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 10:31:00.38691	0
2738	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 10:32:00.437525	0
2739	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 10:33:00.520529	0
2740	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 10:34:00.601458	0
2741	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 10:35:00.682027	0
2742	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 10:36:00.806025	0
2743	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 10:37:00.898735	0
2744	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 10:38:00.978453	0
2745	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 10:39:01.065024	0
2746	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 10:40:01.1814	0
2747	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 10:41:01.263972	0
2748	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 10:42:01.367715	0
2749	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 10:43:01.488914	0
2750	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 10:44:01.581789	0
2751	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 10:45:01.708661	0
2752	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 10:46:01.800021	0
2753	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 10:47:01.89061	0
2754	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 10:48:02.002162	0
2755	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 10:49:02.09418	0
2756	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 10:50:02.196586	0
2757	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 10:51:02.263752	0
2758	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 10:52:02.311836	0
2759	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 10:53:02.442727	0
2760	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 10:54:02.511961	0
2761	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 10:55:02.562948	0
2762	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 11:06:52.97276	0
2763	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 11:07:53.071218	0
2764	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 11:08:53.156546	0
2765	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 11:09:53.257975	0
2766	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 11:10:53.33715	0
2767	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 11:11:53.467111	0
2768	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 11:12:53.575759	0
2769	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 11:13:53.68753	0
2770	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 11:14:53.883125	0
2771	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 11:15:54.047381	0
2772	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 11:16:54.122062	0
2773	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 11:17:54.209736	0
2774	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 11:18:54.294378	0
2775	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 11:19:54.43721	0
2776	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 11:20:54.547993	0
2777	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 11:21:54.621155	0
2778	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 11:22:54.70311	0
2779	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 11:23:54.810333	0
2780	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 11:24:54.893517	0
2781	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 11:25:55.008124	0
2782	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 11:26:55.115992	0
2783	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 11:27:55.232319	0
2784	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 11:28:55.294323	0
2785	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 11:29:55.373631	0
2786	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 11:30:55.464823	0
2787	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 11:31:55.552957	0
2788	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 11:32:55.635335	0
2789	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 11:33:55.726267	0
2790	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 11:34:55.813759	0
2791	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 11:35:55.915925	0
2792	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 11:36:56.009546	0
2793	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 11:37:56.092735	0
2794	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 11:38:56.18824	0
2795	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 11:39:56.283214	0
2796	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 11:40:56.374309	0
2797	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 11:41:56.441238	0
2798	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 11:42:56.533061	0
2799	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 11:43:56.618173	0
2800	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 11:44:56.688598	0
2801	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 11:45:56.776405	0
2802	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 11:46:56.880704	0
2803	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 11:47:56.987152	0
2804	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 11:48:57.063549	0
2805	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 11:49:57.149223	0
2806	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 11:50:57.232975	0
2807	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 11:51:57.338501	0
2808	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 11:52:57.450203	0
2809	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 11:53:57.559316	0
2810	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 11:54:57.643295	0
2811	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 11:55:57.73228	0
2812	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 11:56:57.848486	0
2813	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 11:57:57.959007	0
2814	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 11:58:58.04686	0
2815	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 11:59:58.14836	0
2816	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 12:00:58.250361	0
2817	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 12:01:58.367412	0
2818	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 12:02:58.485655	0
2819	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 12:03:58.615741	0
2820	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 12:04:58.745472	0
2821	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 12:05:58.865686	0
2822	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 12:06:58.992257	0
2823	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 12:07:59.094615	0
2824	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 12:08:59.212317	0
2825	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 12:09:59.333106	0
2826	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 12:10:59.434319	0
2827	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 12:11:59.564616	0
2828	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 12:12:59.647174	0
2829	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 12:13:59.754304	0
2830	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 12:14:59.908157	0
2831	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 12:16:00.015755	0
2832	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 12:17:00.142759	0
2833	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 12:18:00.259504	0
2834	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 12:19:00.341968	0
2835	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 12:20:00.467952	0
2836	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 12:21:00.55474	0
2837	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 12:22:00.659041	0
2838	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 12:23:00.774251	0
2839	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 12:24:00.851644	0
2840	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 12:25:00.911457	0
2841	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 12:26:00.985028	0
2842	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 12:27:01.055851	0
2843	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 12:28:01.115187	0
2844	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 12:29:01.17886	0
2845	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 12:59:52.217968	0
2846	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 13:00:52.327961	0
2847	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 13:01:52.393057	0
2848	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 13:02:52.471408	0
2849	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 13:03:52.587789	0
2850	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 13:04:52.677882	0
2851	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 13:05:52.775849	0
2852	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 13:06:52.884391	0
2853	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 13:07:52.97225	0
2854	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 13:08:53.099537	0
2855	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 13:09:53.197912	0
2856	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 13:10:53.326838	0
2857	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 13:11:53.41406	0
2858	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 13:12:53.497686	0
2859	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 13:13:53.587098	0
2860	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 13:14:53.662591	0
2861	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 13:15:53.780265	0
2862	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 13:16:53.881637	0
2863	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 13:17:53.949549	0
2864	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 13:18:53.981747	0
2865	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 13:23:31.923162	0
2866	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 13:24:32.047021	0
2867	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 13:25:32.153641	0
2868	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 13:26:32.267063	0
2869	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 13:27:32.368475	0
2870	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 13:28:32.469921	0
2871	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 13:29:32.541989	0
2872	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 13:30:32.62646	0
2873	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 13:31:32.729964	0
2874	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 13:32:32.830516	0
2875	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 13:33:32.923857	0
2876	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 13:34:33.041671	0
2877	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 13:35:33.135341	0
2878	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 13:36:33.206743	0
2879	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 13:37:33.300355	0
2880	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 13:38:33.39523	0
2881	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 13:39:33.481882	0
2882	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 13:40:33.584605	0
2883	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 13:41:33.711213	0
2884	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 13:42:33.825584	0
2885	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 13:43:33.925326	0
2886	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 13:44:33.985606	0
2887	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 13:45:34.102561	0
2888	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 13:46:34.198961	0
2889	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 13:47:34.293091	0
2890	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 13:48:34.392386	0
2891	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 13:49:34.493306	0
2892	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 13:50:34.575735	0
2893	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 13:51:34.692884	0
2894	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 13:52:34.777074	0
2895	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 13:53:34.84138	0
2896	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 13:54:34.91643	0
2897	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 13:55:34.985089	0
2898	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 13:56:35.090222	0
2899	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 13:57:35.167893	0
2900	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 13:58:35.262241	0
2901	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 13:59:35.365747	0
2902	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 14:00:35.443102	0
2903	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 14:01:35.543885	0
2904	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 14:02:35.634052	0
2905	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 14:03:35.715907	0
2906	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 14:04:35.776011	0
2907	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 14:05:35.862879	0
2908	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 14:06:35.964387	0
2909	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 14:07:36.034194	0
2910	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 14:08:36.159913	0
2911	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 14:09:36.286767	0
2912	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 14:10:36.417298	0
2913	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 14:11:36.527956	0
2914	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 14:12:36.629788	0
2915	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 14:13:36.725313	0
2916	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 14:14:36.815162	0
2917	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 14:15:36.945256	0
2918	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 14:16:37.043299	0
2919	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 14:17:37.165426	0
2920	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 14:18:37.272496	0
2921	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 14:19:37.337647	0
2922	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 14:20:37.403336	0
2923	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 14:21:37.508765	0
2924	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 14:22:37.630724	0
2925	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 14:23:37.77411	0
2926	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 14:24:37.879289	0
2927	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 14:25:38.008164	0
2928	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 14:26:38.126844	0
2929	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 14:27:38.295615	0
2930	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 14:28:38.37746	0
2931	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 14:29:38.470063	0
2932	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 14:30:38.601574	0
2933	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 14:31:38.726497	0
2934	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 14:32:38.840254	0
2935	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 14:33:38.91876	0
2936	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 14:34:39.041461	0
2937	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 14:35:39.154265	0
2938	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 14:36:39.282177	0
2939	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 14:37:39.372532	0
2940	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 14:38:39.496137	0
2941	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 14:39:39.58904	0
2942	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 14:40:39.71166	0
2943	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 14:41:39.823467	0
2944	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 14:42:39.952313	0
2945	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 14:43:40.058886	0
2946	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 14:44:40.150983	0
2947	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 14:45:40.240215	0
2948	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 14:46:40.371082	0
2949	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 14:47:40.487503	0
2950	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 14:48:40.607878	0
2951	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 14:49:40.68603	0
2952	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 14:50:40.787539	0
2953	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 14:51:40.867176	0
2954	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 14:52:40.958713	0
2955	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 14:53:41.069625	0
2956	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 14:54:41.188295	0
2957	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 14:55:41.296028	0
2958	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 14:56:41.406525	0
2959	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 14:57:41.472947	0
2960	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 14:58:41.557245	0
2961	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 14:59:41.67714	0
2962	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 15:00:41.799466	0
2963	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 15:01:41.911343	0
2964	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 15:02:41.990716	0
2965	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 15:03:42.053954	0
2966	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 15:04:42.167114	0
2967	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 15:05:42.280293	0
2968	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 15:06:42.419885	0
2969	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 15:07:42.518892	0
2970	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 15:08:42.680542	0
2971	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 15:09:42.795969	0
2972	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 15:10:42.88699	0
2973	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 15:11:43.013541	0
2974	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 15:12:43.138398	0
2975	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 15:13:43.267708	0
2976	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 15:14:43.380368	0
2977	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 15:15:43.477752	0
2978	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 15:16:43.589838	0
2979	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 15:17:43.731135	0
2980	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 15:18:43.853699	0
2981	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 15:19:43.9619	0
2982	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 15:20:44.05346	0
2983	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 15:21:44.179204	0
2984	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 15:22:44.277772	0
2985	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 15:23:44.410122	0
2986	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 15:24:44.491799	0
2987	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 15:25:44.560893	0
2988	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 15:26:44.646826	0
2989	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 15:27:44.725	0
2990	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 15:28:44.761577	0
2991	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 15:29:44.783384	0
2992	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 15:30:44.798553	0
2993	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 15:32:41.333113	0
2994	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 15:33:41.404484	0
2995	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 15:34:41.485934	0
2996	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 15:35:41.571052	0
2997	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 15:36:41.631189	0
2998	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 15:37:41.744586	0
2999	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 15:38:41.855926	0
3000	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 15:39:41.966022	0
3001	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 15:40:42.06178	0
3002	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 15:41:42.17439	0
3003	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 15:42:42.269241	0
3004	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 15:43:42.377925	0
3005	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 15:44:42.446354	0
3006	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 15:45:42.564949	0
3007	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 15:46:42.647688	0
3008	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 15:47:42.714322	0
3009	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 15:48:42.818497	0
3010	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 15:49:42.944871	0
3011	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 15:50:43.08824	0
3012	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 15:51:43.233496	0
3013	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 15:52:43.331418	0
3014	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 15:53:43.422526	0
3015	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 15:54:43.530984	0
3016	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 15:55:43.627617	0
3017	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 15:56:43.750132	0
3018	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 15:57:43.836084	0
3019	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 15:58:43.926916	0
3020	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 15:59:44.018107	0
3021	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 16:00:44.089484	0
3022	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 16:01:44.209198	0
3023	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 16:02:44.28849	0
3024	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 16:03:44.393098	0
3025	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 16:04:44.655256	0
3026	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 16:05:44.790255	0
3027	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 16:06:44.884428	0
3028	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 16:07:45.004636	0
3029	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 16:08:45.117463	0
3030	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 16:09:45.18891	0
3031	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 16:10:45.267081	0
3032	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 16:11:45.353999	0
3033	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 16:12:45.429902	0
3034	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 16:13:45.496087	0
3035	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 16:14:45.552401	0
3036	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 16:15:45.687143	0
3037	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 16:16:45.787301	0
3038	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 16:17:45.858197	0
3039	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 16:18:45.940814	0
3040	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 16:19:46.015836	0
3041	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 16:20:46.178671	0
3042	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 16:21:46.27785	0
3043	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 16:22:46.347862	0
3044	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 16:23:46.447544	0
3045	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 16:24:46.576793	0
3046	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 16:25:46.711292	0
3047	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 16:26:46.85158	0
3048	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 16:27:47.001338	0
3049	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 16:28:47.072364	0
3050	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 16:29:47.186284	0
3051	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 16:30:47.277164	0
3052	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 16:31:47.406965	0
3053	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 16:32:47.514989	0
3054	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 16:33:47.592871	0
3055	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 16:34:47.711128	0
3056	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 16:35:47.805911	0
3057	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 16:36:47.907427	0
3058	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 16:37:47.997025	0
3059	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 16:38:48.120234	0
3060	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 16:39:48.237457	0
3061	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 16:40:48.352787	0
3062	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 16:41:48.444614	0
3063	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 16:42:48.506285	0
3064	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 16:43:48.634944	0
3065	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 16:44:48.751176	0
3066	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 16:45:48.851734	0
3067	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 16:46:48.920193	0
3068	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 16:47:49.015978	0
3069	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 16:48:49.099984	0
3070	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 16:49:49.237173	0
3071	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 16:50:49.345422	0
3072	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 16:51:49.437481	0
3073	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 16:52:49.51426	0
3074	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 16:53:49.658237	0
3075	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 16:54:49.775897	0
3076	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 16:55:49.904003	0
3077	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 16:56:50.007653	0
3078	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 16:57:50.107116	0
3079	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 16:58:50.201459	0
3080	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 16:59:50.247229	0
3081	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 17:00:50.38512	0
3082	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 17:01:50.536064	0
3083	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 17:02:50.661144	0
3084	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 17:03:50.775787	0
3085	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 17:04:50.883888	0
3086	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 17:05:50.965378	0
3087	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 17:06:51.076706	0
3088	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 17:07:51.19216	0
3089	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 17:08:51.286865	0
3090	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 17:09:51.408995	0
3091	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 17:10:51.534832	0
3092	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 17:11:51.645301	0
3093	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 17:12:51.72143	0
3094	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 17:13:51.818163	0
3095	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 17:14:51.942949	0
3096	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 17:15:52.03014	0
3097	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 17:16:52.157153	0
3098	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 17:17:52.254837	0
3099	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 17:18:52.361552	0
3100	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 17:19:52.443998	0
3101	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 17:20:52.589502	0
3102	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 17:21:52.690934	0
3103	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 17:22:52.91149	0
3104	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 17:23:53.065771	0
3105	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 17:24:53.133945	0
3106	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 17:25:53.230726	0
3107	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 17:26:53.31146	0
3108	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 17:27:53.452084	0
3109	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 17:28:53.556096	0
3110	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 17:29:53.689031	0
3111	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 17:30:53.802624	0
3112	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 17:31:53.918198	0
3113	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 17:32:53.966741	0
3114	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 17:33:54.04725	0
3115	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 17:34:54.129778	0
3116	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 17:35:54.18643	0
3117	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 17:36:54.299784	0
3118	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 17:37:54.392134	0
3119	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 17:38:54.484856	0
3120	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 17:39:54.603467	0
3121	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 17:40:54.689394	0
3122	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 17:41:54.797097	0
3123	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 17:42:54.906912	0
3124	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 17:43:55.053067	0
3125	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 17:44:55.130189	0
3126	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 17:45:55.206891	0
3127	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 17:46:55.306894	0
3128	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 17:47:55.413713	0
3129	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 17:48:55.529068	0
3130	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 17:49:55.639898	0
3131	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 17:50:55.800808	0
3132	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 17:51:55.903918	0
3133	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 17:52:56.042566	0
3134	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 17:53:56.141634	0
3135	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 17:54:56.26794	0
3136	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 17:55:56.388639	0
3137	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 17:56:56.499212	0
3138	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 17:57:56.640182	0
3139	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 17:58:56.747561	0
3140	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 17:59:56.849677	0
3141	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 18:00:56.939305	0
3142	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 18:01:57.038251	0
3143	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-21 18:02:57.140248	0
3144	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 08:42:47.235458	0
3145	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 08:43:47.338124	0
3146	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 08:44:47.446535	0
3147	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 08:45:47.525317	0
3148	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 08:46:47.614388	0
3149	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 08:47:47.692055	0
3150	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 08:48:47.76959	0
3151	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 08:49:47.858553	0
3152	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 08:50:47.9316	0
3153	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 08:51:48.01946	0
3154	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 08:52:48.093512	0
3155	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 08:53:48.160581	0
3156	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 08:54:48.22324	0
3157	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 08:55:48.317255	0
3158	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 08:56:48.468219	0
3159	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 08:57:48.56807	0
3160	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 08:58:48.675714	0
3161	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 08:59:48.784162	0
3162	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 09:00:48.857152	0
3163	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 09:01:48.974188	0
3164	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 09:02:49.075861	0
3165	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 09:03:49.198906	0
3166	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 09:04:49.326874	0
3167	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 09:05:49.442631	0
3168	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 09:06:49.542671	0
3169	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 09:07:49.652173	0
3170	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 09:08:49.716377	0
3171	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 09:09:49.814617	0
3172	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 09:10:49.891251	0
3173	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 09:11:49.969574	0
3174	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 09:12:50.030416	0
3175	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 09:13:50.110392	0
3176	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 09:14:50.146329	0
3177	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 09:15:50.220595	0
3178	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 09:16:50.303014	0
3179	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 09:17:50.373189	0
3180	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 09:18:50.488938	0
3181	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 09:19:50.559156	0
3182	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 09:20:50.652779	0
3183	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 09:21:50.737886	0
3184	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 09:22:50.831872	0
3185	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 09:23:50.917305	0
3186	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 09:24:51.020743	0
3187	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 09:25:51.084566	0
3188	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 09:26:51.160238	0
3189	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 09:27:51.252096	0
3190	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 09:28:51.353277	0
3191	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 09:29:51.437999	0
3192	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 09:30:51.521208	0
3193	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 09:31:51.594571	0
3194	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 09:32:51.677134	0
3195	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 09:33:51.736341	0
3196	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 09:34:51.807306	0
3197	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 09:35:51.898472	0
3198	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 09:36:51.987498	0
3199	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 09:37:52.072184	0
3200	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 09:38:52.168706	0
3201	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 09:39:52.277784	0
3202	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 09:40:52.361462	0
3203	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 09:41:52.421221	0
3204	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 09:42:52.529296	0
3205	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 09:43:52.595653	0
3206	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 09:44:52.69248	0
3207	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 09:45:52.78063	0
3208	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 09:46:52.899527	0
3209	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 09:47:52.980513	0
3210	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 09:48:53.047388	0
3211	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 09:49:53.107276	0
3212	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 09:50:53.21097	0
3213	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 09:51:53.289153	0
3214	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 09:52:53.376558	0
3215	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 09:53:53.46313	0
3216	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 09:54:53.595506	0
3217	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 09:55:53.678227	0
3218	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 09:56:53.786249	0
3219	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 09:57:53.87782	0
3220	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 09:58:53.979715	0
3221	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 09:59:54.066619	0
3222	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 10:00:54.160761	0
3223	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 10:01:54.250406	0
3224	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 10:02:54.343372	0
3225	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 10:03:54.443331	0
3226	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 10:04:54.514443	0
3227	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 10:05:54.578222	0
3228	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 10:06:54.673578	0
3229	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 10:07:54.755417	0
3230	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 10:08:54.831553	0
3231	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 10:09:54.914114	0
3232	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 10:10:55.022671	0
3233	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 10:11:55.351958	0
3234	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 10:12:55.457648	0
3235	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 10:13:55.582074	0
3236	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 10:14:55.683595	0
3237	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 10:15:55.769885	0
3238	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 10:16:55.851776	0
3239	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 10:17:55.919043	0
3240	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 10:18:55.996277	0
3241	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 10:19:56.085239	0
3242	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 10:20:56.18695	0
3243	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 10:21:56.371292	0
3244	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 10:22:56.447932	0
3245	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 10:23:56.534614	0
3246	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 10:24:56.567701	0
3247	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 10:25:56.629896	0
3248	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 10:26:56.707025	0
3249	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 10:27:56.767754	0
3250	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 10:28:56.869636	0
3251	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 10:29:56.948325	0
3252	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 10:30:57.069229	0
3253	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 10:31:57.140364	0
3254	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 10:32:57.365255	0
3255	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 10:33:57.462248	0
3256	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 10:34:57.538556	0
3257	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 10:35:57.636993	0
3258	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 10:36:57.722389	0
3259	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 10:37:57.800312	0
3260	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 10:38:57.91084	0
3261	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 10:39:58.007567	0
3262	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 10:40:58.076111	0
3263	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 10:41:58.195908	0
3264	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 10:42:58.290418	0
3265	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 10:43:58.404588	0
3266	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 10:44:58.527983	0
3267	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 10:45:58.641477	0
3268	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 10:46:58.764857	0
3269	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 10:47:58.82585	0
3270	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 10:48:58.944367	0
3271	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 10:49:59.013084	0
3272	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 10:50:59.085855	0
3273	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 10:51:59.160895	0
3274	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 10:52:59.254106	0
3275	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 10:53:59.31218	0
3276	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 10:54:59.376562	0
3277	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 10:55:59.437875	0
3278	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 10:56:59.518331	0
3279	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 10:57:59.605767	0
3280	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 10:58:59.724493	0
3281	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 10:59:59.798919	0
3282	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 11:00:59.891473	0
3283	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 11:01:59.969671	0
3284	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 11:03:00.04008	0
3285	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 11:04:00.132739	0
3286	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 11:05:00.214629	0
3287	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 11:06:00.32748	0
3288	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 11:07:00.404458	0
3289	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 11:08:00.464903	0
3290	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 11:09:00.543566	0
3291	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 11:10:00.62959	0
3292	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 11:11:00.715829	0
3293	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 11:12:00.840354	0
3294	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 11:13:00.938911	0
3295	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 11:14:01.015591	0
3296	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 11:15:01.074765	0
3297	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 11:16:01.185194	0
3298	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 11:17:01.284502	0
3299	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 11:18:01.346454	0
3300	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 11:19:01.424545	0
3301	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 11:20:01.531216	0
3302	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 11:21:01.617788	0
3303	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 11:22:01.672096	0
3304	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 11:23:01.776532	0
3305	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 11:24:01.927384	0
3306	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 11:25:01.988212	0
3307	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 11:26:02.066258	0
3308	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 11:27:02.180004	0
3309	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 11:28:02.277888	0
3310	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 11:29:02.381482	0
3311	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 11:30:02.436026	0
3312	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 11:31:02.522876	0
3313	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 11:32:02.611277	0
3314	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 11:33:02.722948	0
3315	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 11:34:02.790227	0
3316	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 11:35:02.859128	0
3317	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 11:36:02.986395	0
3318	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 11:37:03.088232	0
3319	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 11:38:03.184558	0
3320	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 11:39:03.254194	0
3321	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 11:40:03.32689	0
3322	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 11:41:03.366478	0
3323	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 11:42:03.412792	0
3324	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 11:43:03.459429	0
3325	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 11:44:03.558866	0
3326	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 11:45:03.617516	0
3327	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 11:53:59.76195	0
3328	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 11:54:59.840969	0
3329	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 11:55:59.922087	0
3330	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 11:57:00.031279	0
3331	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 11:58:00.143929	0
3332	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 11:59:00.299242	0
3333	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 12:00:00.43195	0
3334	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 12:01:00.546989	0
3335	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 12:02:00.622042	0
3336	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 12:03:00.732832	0
3337	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 12:04:00.826167	0
3338	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 12:05:01.002708	0
3339	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 12:06:01.103986	0
3340	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 12:07:01.163225	0
3341	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 12:08:01.265257	0
3342	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 12:09:01.341005	0
3343	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 12:10:01.42997	0
3344	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 12:11:01.54728	0
3345	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 12:12:01.615766	0
3346	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 12:13:01.724188	0
3347	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 12:14:01.837146	0
3348	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 12:15:01.952994	0
3349	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 12:16:02.029649	0
3350	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 12:17:02.130739	0
3351	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 12:18:02.227743	0
3352	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 12:19:02.321485	0
3353	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 12:20:02.404833	0
3354	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 12:21:02.507356	0
3355	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 12:22:02.544671	0
3356	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 12:23:02.602694	0
3357	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 12:24:02.632259	0
3358	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 12:25:02.657075	0
3359	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 12:26:02.7502	0
3360	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 12:59:00.535758	0
3361	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 13:00:00.665023	0
3362	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 13:01:00.739844	0
3363	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 13:02:00.843716	0
3364	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 13:03:00.92636	0
3365	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 13:04:01.015273	0
3366	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 13:05:01.150653	0
3367	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 13:06:01.23731	0
3368	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 13:07:01.321077	0
3369	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 13:08:01.392805	0
3370	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 13:09:01.433471	0
3371	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 13:10:01.522019	0
3372	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 13:11:01.634084	0
3373	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 13:12:01.746562	0
3374	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 13:13:01.83446	0
3375	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 13:14:01.912109	0
3376	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 13:15:01.993708	0
3377	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 13:16:02.069237	0
3378	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 13:17:02.172402	0
3379	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 13:18:02.28026	0
3380	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 13:19:02.363053	0
3381	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 13:20:02.438063	0
3382	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 13:21:02.560401	0
3383	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 13:22:02.673817	0
3384	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 13:23:02.754507	0
3385	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 13:24:02.84087	0
3386	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 13:25:02.907991	0
3387	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 13:26:02.980565	0
3388	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 13:27:03.056833	0
3389	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 13:28:03.148574	0
3390	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 13:29:03.210459	0
3391	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 13:30:03.286233	0
3392	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 13:31:03.389442	0
3393	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 13:32:03.489836	0
3394	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 13:33:03.56494	0
3395	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 13:34:03.644886	0
3396	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 13:35:03.731321	0
3397	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 13:36:03.80453	0
3398	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 13:37:03.901648	0
3399	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 13:38:04.03454	0
3400	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 13:39:04.118842	0
3401	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 13:40:04.201274	0
3402	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 13:41:04.280975	0
3403	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 13:42:04.385731	0
3404	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 13:43:04.461844	0
3405	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 13:44:04.584317	0
3406	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 13:45:04.710146	0
3407	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 13:46:04.795361	0
3408	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 13:47:04.863771	0
3409	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 13:48:04.937882	0
3410	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 13:49:05.032963	0
3411	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 13:50:05.143018	0
3412	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 13:51:05.235639	0
3413	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 13:52:05.307735	0
3414	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 13:53:05.384035	0
3415	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 13:54:05.456056	0
3416	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 13:55:05.53722	0
3417	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 13:56:05.629561	0
3418	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 13:57:05.680655	0
3419	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 13:58:05.76885	0
3420	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 13:59:05.832389	0
3421	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 14:00:05.904727	0
3422	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 14:01:06.001818	0
3423	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 14:02:06.106627	0
3424	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 14:03:06.184559	0
3425	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 14:04:06.261732	0
3426	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 14:05:06.375292	0
3427	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 14:06:06.605037	0
3428	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 14:07:06.702769	0
3429	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 14:08:06.810981	0
3430	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 14:09:06.894792	0
3431	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 14:10:06.98043	0
3432	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 14:11:07.060684	0
3433	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 14:12:07.15001	0
3434	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 14:13:07.280387	0
3435	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 14:14:07.360028	0
3436	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 14:15:07.460873	0
3437	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 14:16:07.574751	0
3438	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 14:17:07.648922	0
3439	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 14:18:07.731893	0
3440	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 14:19:07.816583	0
3441	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 14:20:07.930617	0
3442	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 14:21:08.010377	0
3443	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 14:22:08.08259	0
3444	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 14:23:08.165713	0
3445	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 14:24:08.27139	0
3446	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 14:25:08.374054	0
3447	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 14:26:08.443949	0
3448	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 14:27:08.528955	0
3449	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 14:28:08.648541	0
3450	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 14:29:08.742524	0
3451	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 14:30:08.8145	0
3452	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 14:31:08.918791	0
3453	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 14:32:09.005713	0
3454	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 14:33:09.099437	0
3455	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 14:34:09.228693	0
3456	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 14:35:09.347271	0
3457	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 14:36:09.475573	0
3458	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 14:37:09.592905	0
3459	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 14:38:09.705654	0
3460	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 14:39:09.76415	0
3461	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 14:40:09.880988	0
3462	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 14:41:09.959818	0
3463	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 14:42:10.098438	0
3464	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 14:43:10.226193	0
3465	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 14:44:10.313318	0
3466	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 14:45:10.398516	0
3467	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 14:46:10.483319	0
3468	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 14:47:10.583365	0
3469	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 14:48:10.660704	0
3470	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 14:49:10.777635	0
3471	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 14:50:10.883656	0
3472	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 14:51:10.990045	0
3473	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 14:52:11.134704	0
3474	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 14:53:11.192579	0
3475	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 14:54:11.315311	0
3476	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 14:55:11.438586	0
3477	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 14:56:11.544124	0
3478	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 14:57:11.647699	0
3479	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 14:58:11.720062	0
3480	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 14:59:11.8088	0
3481	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 15:00:11.906806	0
3482	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 15:01:11.99703	0
3483	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 15:02:12.09641	0
3484	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 15:03:12.16471	0
3485	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 15:04:12.294861	0
3486	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 15:05:12.369713	0
3487	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 15:06:12.510939	0
3488	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 15:07:12.627955	0
3489	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 15:08:12.740127	0
3490	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 15:09:12.843484	0
3491	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 15:10:12.94478	0
3492	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 15:11:13.028253	0
3493	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 15:12:13.128747	0
3494	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 15:13:13.233155	0
3495	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 15:14:13.337876	0
3496	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 15:15:13.449138	0
3497	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 15:16:13.533308	0
3498	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 15:17:13.619881	0
3499	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 15:18:13.738567	0
3500	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 15:19:13.816425	0
3501	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 15:20:13.901872	0
3502	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 15:21:13.984196	0
3503	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 15:22:14.086017	0
3504	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 15:23:14.174096	0
3505	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 15:24:14.28061	0
3506	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 15:25:14.331142	0
3507	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 15:26:14.360673	0
3508	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 15:27:14.396435	0
3509	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 15:28:14.476334	0
3510	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 15:29:14.508935	0
3511	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 15:30:15.211418	0
3512	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 15:31:15.277837	0
3513	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 15:32:15.460115	0
3514	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 15:33:15.546677	0
3515	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 15:34:15.672524	0
3516	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 15:35:15.781452	0
3517	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 15:36:15.907368	0
3518	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 15:37:16.012445	0
3519	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 15:38:16.119009	0
3520	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 15:39:16.219984	0
3521	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 15:40:16.323202	0
3522	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 15:41:16.511086	0
3523	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 15:42:16.617405	0
3524	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 15:43:16.703579	0
3525	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 15:44:16.790025	0
3526	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 15:45:16.880223	0
3527	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 15:46:16.990051	0
3528	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 15:47:17.056567	0
3529	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 15:48:17.1583	0
3530	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 15:49:17.251444	0
3531	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 15:50:17.335028	0
3532	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 15:51:17.432064	0
3533	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 15:52:17.549404	0
3534	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 15:53:17.632326	0
3535	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 15:54:17.7447	0
3536	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 15:55:17.838083	0
3537	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 15:56:17.936908	0
3538	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 15:57:18.026201	0
3539	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 15:58:18.119172	0
3540	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 15:59:18.197609	0
3541	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 16:00:18.270399	0
3542	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 16:01:18.350385	0
3543	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 16:02:18.449064	0
3544	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 16:03:18.572157	0
3545	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 16:04:18.698161	0
3546	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 16:05:18.808444	0
3547	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 16:06:18.889793	0
3548	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 16:07:18.984978	0
3549	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 16:08:19.104504	0
3550	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 16:09:19.187744	0
3551	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 16:10:19.293596	0
3552	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 16:11:19.393126	0
3553	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 16:12:19.546338	0
3554	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 16:13:19.645516	0
3555	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 16:14:19.764005	0
3556	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 16:15:19.818976	0
3557	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 16:16:19.941333	0
3558	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 16:17:20.015521	0
3559	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 16:18:20.162997	0
3560	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 16:19:20.287493	0
3561	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 16:20:20.374006	0
3562	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 16:21:20.458509	0
3563	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 16:22:20.576058	0
3564	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 16:23:20.691172	0
3565	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 16:24:20.79723	0
3566	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 16:25:20.900158	0
3567	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 16:26:21.006854	0
3568	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 16:27:21.09954	0
3569	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 16:28:21.197938	0
3570	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 16:29:21.305492	0
3571	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 16:30:21.379002	0
3572	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 16:31:21.490334	0
3573	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 16:32:21.585769	0
3574	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 16:33:21.66604	0
3575	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 16:34:21.776679	0
3576	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 16:35:21.877467	0
3577	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 16:36:21.945185	0
3578	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 16:37:22.051669	0
3579	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 16:38:22.169256	0
3580	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 16:39:22.283175	0
3581	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 16:40:22.397182	0
3582	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 16:41:22.481617	0
3583	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 16:42:22.626285	0
3584	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 16:43:22.739559	0
3585	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 16:44:22.820515	0
3586	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 16:45:22.947022	0
3587	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 16:46:23.038427	0
3588	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 16:47:23.155201	0
3589	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 16:48:23.266751	0
3590	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 16:49:23.374496	0
3591	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 16:50:23.489401	0
3592	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 16:51:23.632057	0
3593	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 16:52:23.750398	0
3594	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 16:53:23.857115	0
3595	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 16:54:23.96575	0
3596	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 16:55:24.0323	0
3597	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 16:56:24.110742	0
3598	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 17:08:15.665406	0
3599	DataLog_HMI1_20250501.csv	01	1	\N	失敗	データ不正の為、データのインポートに失敗しました。	2025-05-22 17:09:15.816735	0
753	1_NV4000_202501.CSV	1	1	23	成功	ファイルのデータは上書きされました。	2025-04-29 16:45:37.818214	1
3600	1_NV4000_202501.CSV		0	\N	失敗	設備Noが存在しない為、データのインポートに失敗しました。	2025-05-22 17:09:15.82389	1
3601	2_NV5000_202501.CSV		0	\N	失敗	設備Noが存在しない為、データのインポートに失敗しました。	2025-05-22 17:10:15.861968	1
3602	4_Enshu450V_202501.CSV		0	\N	失敗	設備Noが存在しない為、データのインポートに失敗しました。	2025-05-22 17:10:15.950367	1
3603	9_NakamuraTome_SC260-1_202501.CSV		0	\N	失敗	設備Noが存在しない為、データのインポートに失敗しました。	2025-05-22 17:11:16.003386	1
747	20_NC-Rooter2_202501.CSV	20	2	23	成功	ファイルのデータは上書きされました。	2025-04-29 11:39:03.69299	1
3604	20_NC-Rooter2_202501.CSV	20	2	23	成功	データのインポートに成功しました。	2025-05-22 17:11:16.086591	1
3605	01_NV4000_202501.CSV	01	1	23	成功	データのインポートに成功しました。	2025-05-22 17:14:16.306798	1
3606	02_NV5000_202501.CSV	02	2	23	成功	データのインポートに成功しました。	2025-05-22 17:14:16.384869	1
3607	09_NakamuraTome_SC260-1_202501.CSV	09	1	23	成功	データのインポートに成功しました。	2025-05-22 17:14:16.450568	1
3608	04_Enshu450V_202501.CSV	04	1	23	成功	データのインポートに成功しました。	2025-05-22 17:15:16.395917	1
\.


--
-- TOC entry 3386 (class 0 OID 15935315)
-- Dependencies: 239
-- Data for Name: TRN_IMPORT_HISTORY_DETAIL; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."TRN_IMPORT_HISTORY_DETAIL" ("ID", "IDDATA", "FACTORY_CD", "MACHINE_NO", "FILENAME", "IMPORTTIME") FROM stdin;
10782	3453	1	1	1_NV4000_202503.CSV	2025-04-23 10:32:31.85217
10783	3454	1	1	1_NV4000_202503.CSV	2025-04-23 10:32:31.85217
10785	3455	1	1	1_NV4000_202503.CSV	2025-04-23 10:32:31.85217
10788	3456	1	1	1_NV4000_202503.CSV	2025-04-23 10:32:31.85217
10789	3457	1	1	1_NV4000_202503.CSV	2025-04-23 10:32:31.85217
10791	3458	1	1	1_NV4000_202503.CSV	2025-04-23 10:32:31.85217
10793	3459	1	1	1_NV4000_202503.CSV	2025-04-23 10:32:31.85217
10795	3460	1	1	1_NV4000_202503.CSV	2025-04-23 10:32:31.85217
10797	3461	1	1	1_NV4000_202503.CSV	2025-04-23 10:32:31.85217
10799	3462	1	1	1_NV4000_202503.CSV	2025-04-23 10:32:31.85217
10801	3463	1	1	1_NV4000_202503.CSV	2025-04-23 10:32:31.85217
10803	3464	1	1	1_NV4000_202503.CSV	2025-04-23 10:32:31.85217
10805	3465	1	1	1_NV4000_202503.CSV	2025-04-23 10:32:31.85217
10807	3466	1	1	1_NV4000_202503.CSV	2025-04-23 10:32:31.85217
10809	3467	1	1	1_NV4000_202503.CSV	2025-04-23 10:32:31.85217
10810	3468	1	1	1_NV4000_202503.CSV	2025-04-23 10:32:31.85217
10811	3469	1	1	1_NV4000_202503.CSV	2025-04-23 10:32:31.85217
10812	3470	1	1	1_NV4000_202503.CSV	2025-04-23 10:32:31.85217
10813	3472	1	1	1_NV4000_202503.CSV	2025-04-23 10:32:31.85217
10814	3473	1	1	1_NV4000_202503.CSV	2025-04-23 10:32:31.85217
10815	3474	1	1	1_NV4000_202503.CSV	2025-04-23 10:32:31.85217
10831	9157	1	1	DataLog_HMI1_20250408.csv	2025-04-23 10:32:45.791868
10832	9158	1	1	DataLog_HMI1_20250408.csv	2025-04-23 10:32:45.791868
10833	9159	1	1	DataLog_HMI1_20250408.csv	2025-04-23 10:32:45.791868
10834	9160	1	1	DataLog_HMI1_20250408.csv	2025-04-23 10:32:45.791868
10835	9161	1	1	DataLog_HMI1_20250408.csv	2025-04-23 10:32:45.791868
10836	9162	1	1	DataLog_HMI1_20250408.csv	2025-04-23 10:32:45.791868
10837	9163	1	1	DataLog_HMI1_20250408.csv	2025-04-23 10:32:45.791868
10838	9164	1	1	DataLog_HMI1_20250408.csv	2025-04-23 10:32:45.791868
10839	9165	1	1	DataLog_HMI1_20250408.csv	2025-04-23 10:32:45.791868
10840	9166	1	1	DataLog_HMI1_20250408.csv	2025-04-23 10:32:45.791868
10841	9167	1	1	DataLog_HMI1_20250408.csv	2025-04-23 10:32:45.791868
10842	9168	1	1	DataLog_HMI1_20250408.csv	2025-04-23 10:32:45.791868
10843	9169	1	1	DataLog_HMI1_20250408.csv	2025-04-23 10:32:45.791868
10844	9170	1	1	DataLog_HMI1_20250408.csv	2025-04-23 10:32:45.791868
10845	9171	1	1	DataLog_HMI1_20250408.csv	2025-04-23 10:32:45.791868
10846	9172	1	1	DataLog_HMI1_20250408.csv	2025-04-23 10:32:45.791868
10847	9173	1	1	DataLog_HMI1_20250408.csv	2025-04-23 10:32:45.791868
10848	9174	1	1	DataLog_HMI1_20250408.csv	2025-04-23 10:32:45.791868
10864	9190	1	1	DataLog_HMI1_20250409.csv	2025-04-23 10:32:45.858106
10865	9191	1	1	DataLog_HMI1_20250409.csv	2025-04-23 10:32:45.858106
10866	9192	1	1	DataLog_HMI1_20250409.csv	2025-04-23 10:32:45.858106
10867	9193	1	1	DataLog_HMI1_20250409.csv	2025-04-23 10:32:45.858106
10868	9194	1	1	DataLog_HMI1_20250409.csv	2025-04-23 10:32:45.858106
10869	9195	1	1	DataLog_HMI1_20250409.csv	2025-04-23 10:32:45.858106
10870	9196	1	1	DataLog_HMI1_20250409.csv	2025-04-23 10:32:45.858106
10871	9197	1	1	DataLog_HMI1_20250409.csv	2025-04-23 10:32:45.858106
10872	9198	1	1	DataLog_HMI1_20250409.csv	2025-04-23 10:32:45.858106
10873	9199	1	1	DataLog_HMI1_20250409.csv	2025-04-23 10:32:45.858106
10874	9200	1	1	DataLog_HMI1_20250409.csv	2025-04-23 10:32:45.858106
10875	9201	1	1	DataLog_HMI1_20250409.csv	2025-04-23 10:32:45.858106
10876	9202	1	1	DataLog_HMI1_20250409.csv	2025-04-23 10:32:45.858106
10877	9203	1	1	DataLog_HMI1_20250409.csv	2025-04-23 10:32:45.858106
10878	9204	1	1	DataLog_HMI1_20250409.csv	2025-04-23 10:32:45.858106
10879	9205	1	1	DataLog_HMI1_20250409.csv	2025-04-23 10:32:45.858106
10895	3488	2	2	2_NV5000_202503.CSV	2025-04-23 10:33:05.258469
10896	3489	2	2	2_NV5000_202503.CSV	2025-04-23 10:33:05.258469
10897	3490	2	2	2_NV5000_202503.CSV	2025-04-23 10:33:05.258469
10898	3491	2	2	2_NV5000_202503.CSV	2025-04-23 10:33:05.258469
10899	3492	2	2	2_NV5000_202503.CSV	2025-04-23 10:33:05.258469
10900	3493	2	2	2_NV5000_202503.CSV	2025-04-23 10:33:05.258469
10901	3494	2	2	2_NV5000_202503.CSV	2025-04-23 10:33:05.258469
10902	3495	2	2	2_NV5000_202503.CSV	2025-04-23 10:33:05.258469
10903	3496	2	2	2_NV5000_202503.CSV	2025-04-23 10:33:05.258469
10904	3497	2	2	2_NV5000_202503.CSV	2025-04-23 10:33:05.258469
10905	3498	2	2	2_NV5000_202503.CSV	2025-04-23 10:33:05.258469
10906	3499	2	2	2_NV5000_202503.CSV	2025-04-23 10:33:05.258469
10907	3500	2	2	2_NV5000_202503.CSV	2025-04-23 10:33:05.258469
10908	3501	2	2	2_NV5000_202503.CSV	2025-04-23 10:33:05.258469
10909	3502	2	2	2_NV5000_202503.CSV	2025-04-23 10:33:05.258469
10910	3503	2	2	2_NV5000_202503.CSV	2025-04-23 10:33:05.258469
10911	3504	2	2	2_NV5000_202503.CSV	2025-04-23 10:33:05.258469
10912	3505	2	2	2_NV5000_202503.CSV	2025-04-23 10:33:05.258469
10913	3506	2	2	2_NV5000_202503.CSV	2025-04-23 10:33:05.258469
10914	3507	2	2	2_NV5000_202503.CSV	2025-04-23 10:33:05.258469
10915	3508	2	2	2_NV5000_202503.CSV	2025-04-23 10:33:05.258469
10931	9221	2	2	DataLog_HMI2_20250401.csv	2025-04-23 10:33:14.746838
10932	9222	2	2	DataLog_HMI2_20250401.csv	2025-04-23 10:33:14.746838
10933	9223	2	2	DataLog_HMI2_20250401.csv	2025-04-23 10:33:14.746838
10934	9224	2	2	DataLog_HMI2_20250401.csv	2025-04-23 10:33:14.746838
10935	9225	2	2	DataLog_HMI2_20250401.csv	2025-04-23 10:33:14.746838
10936	9226	2	2	DataLog_HMI2_20250401.csv	2025-04-23 10:33:14.746838
10937	9227	2	2	DataLog_HMI2_20250401.csv	2025-04-23 10:33:14.746838
10938	9228	2	2	DataLog_HMI2_20250401.csv	2025-04-23 10:33:14.746838
10939	9229	2	2	DataLog_HMI2_20250401.csv	2025-04-23 10:33:14.746838
10940	9230	2	2	DataLog_HMI2_20250401.csv	2025-04-23 10:33:14.746838
10941	9231	2	2	DataLog_HMI2_20250401.csv	2025-04-23 10:33:14.746838
10942	9232	2	2	DataLog_HMI2_20250401.csv	2025-04-23 10:33:14.746838
10943	9233	2	2	DataLog_HMI2_20250401.csv	2025-04-23 10:33:14.746838
10944	9234	2	2	DataLog_HMI2_20250401.csv	2025-04-23 10:33:14.746838
10945	9235	2	2	DataLog_HMI2_20250401.csv	2025-04-23 10:33:14.746838
10946	9236	2	2	DataLog_HMI2_20250401.csv	2025-04-23 10:33:14.746838
10781	3471	1	1	1_NV4000_202504.CSV	2025-04-23 10:32:31.858539
10784	3475	1	1	1_NV4000_202504.CSV	2025-04-23 10:32:31.858539
10786	3476	1	1	1_NV4000_202504.CSV	2025-04-23 10:32:31.858539
10787	3477	1	1	1_NV4000_202504.CSV	2025-04-23 10:32:31.858539
10790	3478	1	1	1_NV4000_202504.CSV	2025-04-23 10:32:31.858539
10792	3479	1	1	1_NV4000_202504.CSV	2025-04-23 10:32:31.858539
10794	3480	1	1	1_NV4000_202504.CSV	2025-04-23 10:32:31.858539
10796	3481	1	1	1_NV4000_202504.CSV	2025-04-23 10:32:31.858539
10798	3482	1	1	1_NV4000_202504.CSV	2025-04-23 10:32:31.858539
10800	3483	1	1	1_NV4000_202504.CSV	2025-04-23 10:32:31.858539
10802	3484	1	1	1_NV4000_202504.CSV	2025-04-23 10:32:31.858539
10804	3485	1	1	1_NV4000_202504.CSV	2025-04-23 10:32:31.858539
10806	3486	1	1	1_NV4000_202504.CSV	2025-04-23 10:32:31.858539
10808	3487	1	1	1_NV4000_202504.CSV	2025-04-23 10:32:31.858539
10816	9142	1	1	DataLog_HMI1_20250302.csv	2025-04-23 10:32:45.739553
10817	9143	1	1	DataLog_HMI1_20250302.csv	2025-04-23 10:32:45.739553
10818	9144	1	1	DataLog_HMI1_20250302.csv	2025-04-23 10:32:45.739553
10819	9145	1	1	DataLog_HMI1_20250302.csv	2025-04-23 10:32:45.739553
10820	9146	1	1	DataLog_HMI1_20250302.csv	2025-04-23 10:32:45.739553
10821	9147	1	1	DataLog_HMI1_20250302.csv	2025-04-23 10:32:45.739553
10822	9148	1	1	DataLog_HMI1_20250302.csv	2025-04-23 10:32:45.739553
10823	9149	1	1	DataLog_HMI1_20250302.csv	2025-04-23 10:32:45.739553
10824	9150	1	1	DataLog_HMI1_20250302.csv	2025-04-23 10:32:45.739553
10825	9151	1	1	DataLog_HMI1_20250302.csv	2025-04-23 10:32:45.739553
10826	9152	1	1	DataLog_HMI1_20250302.csv	2025-04-23 10:32:45.739553
10827	9153	1	1	DataLog_HMI1_20250302.csv	2025-04-23 10:32:45.739553
10828	9154	1	1	DataLog_HMI1_20250302.csv	2025-04-23 10:32:45.739553
10829	9155	1	1	DataLog_HMI1_20250302.csv	2025-04-23 10:32:45.739553
10830	9156	1	1	DataLog_HMI1_20250302.csv	2025-04-23 10:32:45.739553
10880	9206	1	1	DataLog_HMI1_20250301.csv	2025-04-23 10:32:45.897835
10881	9207	1	1	DataLog_HMI1_20250301.csv	2025-04-23 10:32:45.897835
10882	9208	1	1	DataLog_HMI1_20250301.csv	2025-04-23 10:32:45.897835
10883	9209	1	1	DataLog_HMI1_20250301.csv	2025-04-23 10:32:45.897835
10884	9210	1	1	DataLog_HMI1_20250301.csv	2025-04-23 10:32:45.897835
10885	9211	1	1	DataLog_HMI1_20250301.csv	2025-04-23 10:32:45.897835
10886	9212	1	1	DataLog_HMI1_20250301.csv	2025-04-23 10:32:45.897835
10887	9213	1	1	DataLog_HMI1_20250301.csv	2025-04-23 10:32:45.897835
10888	9214	1	1	DataLog_HMI1_20250301.csv	2025-04-23 10:32:45.897835
10889	9215	1	1	DataLog_HMI1_20250301.csv	2025-04-23 10:32:45.897835
10890	9216	1	1	DataLog_HMI1_20250301.csv	2025-04-23 10:32:45.897835
10891	9217	1	1	DataLog_HMI1_20250301.csv	2025-04-23 10:32:45.897835
10892	9218	1	1	DataLog_HMI1_20250301.csv	2025-04-23 10:32:45.897835
10893	9219	1	1	DataLog_HMI1_20250301.csv	2025-04-23 10:32:45.897835
10894	9220	1	1	DataLog_HMI1_20250301.csv	2025-04-23 10:32:45.897835
10916	3509	2	2	2_NV5000_202504.CSV	2025-04-23 10:33:05.29617
10917	3510	2	2	2_NV5000_202504.CSV	2025-04-23 10:33:05.29617
10918	3511	2	2	2_NV5000_202504.CSV	2025-04-23 10:33:05.29617
10919	3512	2	2	2_NV5000_202504.CSV	2025-04-23 10:33:05.29617
10920	3513	2	2	2_NV5000_202504.CSV	2025-04-23 10:33:05.29617
10921	3514	2	2	2_NV5000_202504.CSV	2025-04-23 10:33:05.29617
10922	3515	2	2	2_NV5000_202504.CSV	2025-04-23 10:33:05.29617
10923	3516	2	2	2_NV5000_202504.CSV	2025-04-23 10:33:05.29617
10924	3517	2	2	2_NV5000_202504.CSV	2025-04-23 10:33:05.29617
10925	3518	2	2	2_NV5000_202504.CSV	2025-04-23 10:33:05.29617
10926	3519	2	2	2_NV5000_202504.CSV	2025-04-23 10:33:05.29617
10927	3520	2	2	2_NV5000_202504.CSV	2025-04-23 10:33:05.29617
10928	3521	2	2	2_NV5000_202504.CSV	2025-04-23 10:33:05.29617
10929	3522	2	2	2_NV5000_202504.CSV	2025-04-23 10:33:05.29617
10930	3523	2	2	2_NV5000_202504.CSV	2025-04-23 10:33:05.29617
10969	9259	2	2	DataLog_HMI2_20250411.csv	2025-04-23 10:33:14.817237
10970	9260	2	2	DataLog_HMI2_20250411.csv	2025-04-23 10:33:14.817237
10971	9261	2	2	DataLog_HMI2_20250411.csv	2025-04-23 10:33:14.817237
10972	9262	2	2	DataLog_HMI2_20250411.csv	2025-04-23 10:33:14.817237
10973	9263	2	2	DataLog_HMI2_20250411.csv	2025-04-23 10:33:14.817237
10974	9264	2	2	DataLog_HMI2_20250411.csv	2025-04-23 10:33:14.817237
10975	9265	2	2	DataLog_HMI2_20250411.csv	2025-04-23 10:33:14.817237
10976	9266	2	2	DataLog_HMI2_20250411.csv	2025-04-23 10:33:14.817237
10977	9267	2	2	DataLog_HMI2_20250411.csv	2025-04-23 10:33:14.817237
10978	9268	2	2	DataLog_HMI2_20250411.csv	2025-04-23 10:33:14.817237
10979	9269	2	2	DataLog_HMI2_20250411.csv	2025-04-23 10:33:14.817237
10980	9270	2	2	DataLog_HMI2_20250411.csv	2025-04-23 10:33:14.817237
10981	9271	2	2	DataLog_HMI2_20250411.csv	2025-04-23 10:33:14.817237
10982	9272	2	2	DataLog_HMI2_20250411.csv	2025-04-23 10:33:14.817237
10983	9273	2	2	DataLog_HMI2_20250414.csv	2025-04-23 10:33:14.847544
10984	9274	2	2	DataLog_HMI2_20250414.csv	2025-04-23 10:33:14.847544
10985	9275	2	2	DataLog_HMI2_20250414.csv	2025-04-23 10:33:14.847544
10986	9276	2	2	DataLog_HMI2_20250414.csv	2025-04-23 10:33:14.847544
10987	9277	2	2	DataLog_HMI2_20250414.csv	2025-04-23 10:33:14.847544
10988	9278	2	2	DataLog_HMI2_20250414.csv	2025-04-23 10:33:14.847544
10989	9279	2	2	DataLog_HMI2_20250414.csv	2025-04-23 10:33:14.847544
10990	9280	2	2	DataLog_HMI2_20250414.csv	2025-04-23 10:33:14.847544
10991	9281	2	2	DataLog_HMI2_20250414.csv	2025-04-23 10:33:14.847544
10992	9282	2	2	DataLog_HMI2_20250414.csv	2025-04-23 10:33:14.847544
10993	9283	2	2	DataLog_HMI2_20250414.csv	2025-04-23 10:33:14.847544
10994	9284	2	2	DataLog_HMI2_20250414.csv	2025-04-23 10:33:14.847544
11020	9310	2	2	DataLog_HMI2_20250417.csv	2025-04-23 10:33:14.959955
11021	9311	2	2	DataLog_HMI2_20250417.csv	2025-04-23 10:33:14.959955
11022	9312	2	2	DataLog_HMI2_20250417.csv	2025-04-23 10:33:14.959955
11023	9313	2	2	DataLog_HMI2_20250417.csv	2025-04-23 10:33:14.959955
11024	9314	2	2	DataLog_HMI2_20250417.csv	2025-04-23 10:33:14.959955
14499	5181	2	20	20_NC-Rooter2_202501.CSV	2025-05-22 17:11:16.086591
10849	9175	1	1	DataLog_HMI1_20250401.csv	2025-04-23 10:32:45.739672
10850	9176	1	1	DataLog_HMI1_20250401.csv	2025-04-23 10:32:45.739672
10851	9177	1	1	DataLog_HMI1_20250401.csv	2025-04-23 10:32:45.739672
10852	9178	1	1	DataLog_HMI1_20250401.csv	2025-04-23 10:32:45.739672
10853	9179	1	1	DataLog_HMI1_20250401.csv	2025-04-23 10:32:45.739672
10854	9180	1	1	DataLog_HMI1_20250401.csv	2025-04-23 10:32:45.739672
10855	9181	1	1	DataLog_HMI1_20250401.csv	2025-04-23 10:32:45.739672
10856	9182	1	1	DataLog_HMI1_20250401.csv	2025-04-23 10:32:45.739672
10857	9183	1	1	DataLog_HMI1_20250401.csv	2025-04-23 10:32:45.739672
10858	9184	1	1	DataLog_HMI1_20250401.csv	2025-04-23 10:32:45.739672
10859	9185	1	1	DataLog_HMI1_20250401.csv	2025-04-23 10:32:45.739672
10860	9186	1	1	DataLog_HMI1_20250401.csv	2025-04-23 10:32:45.739672
10861	9187	1	1	DataLog_HMI1_20250401.csv	2025-04-23 10:32:45.739672
10862	9188	1	1	DataLog_HMI1_20250401.csv	2025-04-23 10:32:45.739672
10863	9189	1	1	DataLog_HMI1_20250401.csv	2025-04-23 10:32:45.739672
10949	9239	2	2	DataLog_HMI2_20250410.csv	2025-04-23 10:33:14.781599
10950	9240	2	2	DataLog_HMI2_20250410.csv	2025-04-23 10:33:14.781599
10951	9241	2	2	DataLog_HMI2_20250410.csv	2025-04-23 10:33:14.781599
10952	9242	2	2	DataLog_HMI2_20250410.csv	2025-04-23 10:33:14.781599
10953	9243	2	2	DataLog_HMI2_20250410.csv	2025-04-23 10:33:14.781599
10954	9244	2	2	DataLog_HMI2_20250410.csv	2025-04-23 10:33:14.781599
10955	9245	2	2	DataLog_HMI2_20250410.csv	2025-04-23 10:33:14.781599
10956	9246	2	2	DataLog_HMI2_20250410.csv	2025-04-23 10:33:14.781599
10957	9247	2	2	DataLog_HMI2_20250410.csv	2025-04-23 10:33:14.781599
10958	9248	2	2	DataLog_HMI2_20250410.csv	2025-04-23 10:33:14.781599
10959	9249	2	2	DataLog_HMI2_20250410.csv	2025-04-23 10:33:14.781599
10960	9250	2	2	DataLog_HMI2_20250410.csv	2025-04-23 10:33:14.781599
10961	9251	2	2	DataLog_HMI2_20250410.csv	2025-04-23 10:33:14.781599
10962	9252	2	2	DataLog_HMI2_20250410.csv	2025-04-23 10:33:14.781599
10963	9253	2	2	DataLog_HMI2_20250410.csv	2025-04-23 10:33:14.781599
10964	9254	2	2	DataLog_HMI2_20250410.csv	2025-04-23 10:33:14.781599
10965	9255	2	2	DataLog_HMI2_20250410.csv	2025-04-23 10:33:14.781599
10966	9256	2	2	DataLog_HMI2_20250410.csv	2025-04-23 10:33:14.781599
10967	9257	2	2	DataLog_HMI2_20250410.csv	2025-04-23 10:33:14.781599
10968	9258	2	2	DataLog_HMI2_20250410.csv	2025-04-23 10:33:14.781599
10995	9285	2	2	DataLog_HMI2_20250415.csv	2025-04-23 10:33:14.885474
10996	9286	2	2	DataLog_HMI2_20250415.csv	2025-04-23 10:33:14.885474
10997	9287	2	2	DataLog_HMI2_20250415.csv	2025-04-23 10:33:14.885474
10998	9288	2	2	DataLog_HMI2_20250415.csv	2025-04-23 10:33:14.885474
10999	9289	2	2	DataLog_HMI2_20250415.csv	2025-04-23 10:33:14.885474
11000	9290	2	2	DataLog_HMI2_20250415.csv	2025-04-23 10:33:14.885474
11001	9291	2	2	DataLog_HMI2_20250415.csv	2025-04-23 10:33:14.885474
11002	9292	2	2	DataLog_HMI2_20250415.csv	2025-04-23 10:33:14.885474
11003	9293	2	2	DataLog_HMI2_20250415.csv	2025-04-23 10:33:14.885474
11004	9294	2	2	DataLog_HMI2_20250415.csv	2025-04-23 10:33:14.885474
11005	9295	2	2	DataLog_HMI2_20250415.csv	2025-04-23 10:33:14.885474
14500	5182	2	20	20_NC-Rooter2_202501.CSV	2025-05-22 17:11:16.086591
14501	5183	2	20	20_NC-Rooter2_202501.CSV	2025-05-22 17:11:16.086591
14502	5184	2	20	20_NC-Rooter2_202501.CSV	2025-05-22 17:11:16.086591
14503	5185	2	20	20_NC-Rooter2_202501.CSV	2025-05-22 17:11:16.086591
14504	5186	2	20	20_NC-Rooter2_202501.CSV	2025-05-22 17:11:16.086591
14505	5187	2	20	20_NC-Rooter2_202501.CSV	2025-05-22 17:11:16.086591
14506	5188	2	20	20_NC-Rooter2_202501.CSV	2025-05-22 17:11:16.086591
14507	5189	2	20	20_NC-Rooter2_202501.CSV	2025-05-22 17:11:16.086591
14508	5190	2	20	20_NC-Rooter2_202501.CSV	2025-05-22 17:11:16.086591
14509	5191	2	20	20_NC-Rooter2_202501.CSV	2025-05-22 17:11:16.086591
14510	5192	2	20	20_NC-Rooter2_202501.CSV	2025-05-22 17:11:16.086591
14511	5193	2	20	20_NC-Rooter2_202501.CSV	2025-05-22 17:11:16.086591
14512	5194	2	20	20_NC-Rooter2_202501.CSV	2025-05-22 17:11:16.086591
14513	5195	2	20	20_NC-Rooter2_202501.CSV	2025-05-22 17:11:16.086591
14514	5196	2	20	20_NC-Rooter2_202501.CSV	2025-05-22 17:11:16.086591
14515	5197	2	20	20_NC-Rooter2_202501.CSV	2025-05-22 17:11:16.086591
14516	5198	2	20	20_NC-Rooter2_202501.CSV	2025-05-22 17:11:16.086591
14517	5199	2	20	20_NC-Rooter2_202501.CSV	2025-05-22 17:11:16.086591
14518	5200	2	20	20_NC-Rooter2_202501.CSV	2025-05-22 17:11:16.086591
14519	5201	2	20	20_NC-Rooter2_202501.CSV	2025-05-22 17:11:16.086591
14520	5202	2	20	20_NC-Rooter2_202501.CSV	2025-05-22 17:11:16.086591
14521	5203	2	20	20_NC-Rooter2_202501.CSV	2025-05-22 17:11:16.086591
14522	5204	1	01	01_NV4000_202501.CSV	2025-05-22 17:14:16.306798
14523	5205	1	01	01_NV4000_202501.CSV	2025-05-22 17:14:16.306798
14524	5206	1	01	01_NV4000_202501.CSV	2025-05-22 17:14:16.306798
14525	5207	1	01	01_NV4000_202501.CSV	2025-05-22 17:14:16.306798
14526	5208	1	01	01_NV4000_202501.CSV	2025-05-22 17:14:16.306798
14527	5209	1	01	01_NV4000_202501.CSV	2025-05-22 17:14:16.306798
14528	5210	1	01	01_NV4000_202501.CSV	2025-05-22 17:14:16.306798
14529	5211	1	01	01_NV4000_202501.CSV	2025-05-22 17:14:16.306798
14530	5212	1	01	01_NV4000_202501.CSV	2025-05-22 17:14:16.306798
14531	5213	1	01	01_NV4000_202501.CSV	2025-05-22 17:14:16.306798
14532	5214	1	01	01_NV4000_202501.CSV	2025-05-22 17:14:16.306798
14533	5215	1	01	01_NV4000_202501.CSV	2025-05-22 17:14:16.306798
14534	5216	1	01	01_NV4000_202501.CSV	2025-05-22 17:14:16.306798
14535	5217	1	01	01_NV4000_202501.CSV	2025-05-22 17:14:16.306798
14536	5218	1	01	01_NV4000_202501.CSV	2025-05-22 17:14:16.306798
14537	5219	1	01	01_NV4000_202501.CSV	2025-05-22 17:14:16.306798
14538	5220	1	01	01_NV4000_202501.CSV	2025-05-22 17:14:16.306798
14539	5221	1	01	01_NV4000_202501.CSV	2025-05-22 17:14:16.306798
14540	5222	1	01	01_NV4000_202501.CSV	2025-05-22 17:14:16.306798
14541	5223	1	01	01_NV4000_202501.CSV	2025-05-22 17:14:16.306798
14542	5224	1	01	01_NV4000_202501.CSV	2025-05-22 17:14:16.306798
14543	5225	1	01	01_NV4000_202501.CSV	2025-05-22 17:14:16.306798
10947	9237	2	2	DataLog_HMI2_20250401.csv	2025-04-23 10:33:14.746838
10948	9238	2	2	DataLog_HMI2_20250401.csv	2025-04-23 10:33:14.746838
11006	9296	2	2	DataLog_HMI2_20250416.csv	2025-04-23 10:33:14.921851
11007	9297	2	2	DataLog_HMI2_20250416.csv	2025-04-23 10:33:14.921851
11008	9298	2	2	DataLog_HMI2_20250416.csv	2025-04-23 10:33:14.921851
11009	9299	2	2	DataLog_HMI2_20250416.csv	2025-04-23 10:33:14.921851
11010	9300	2	2	DataLog_HMI2_20250416.csv	2025-04-23 10:33:14.921851
11011	9301	2	2	DataLog_HMI2_20250416.csv	2025-04-23 10:33:14.921851
11012	9302	2	2	DataLog_HMI2_20250416.csv	2025-04-23 10:33:14.921851
11013	9303	2	2	DataLog_HMI2_20250416.csv	2025-04-23 10:33:14.921851
11014	9304	2	2	DataLog_HMI2_20250416.csv	2025-04-23 10:33:14.921851
11015	9305	2	2	DataLog_HMI2_20250416.csv	2025-04-23 10:33:14.921851
11016	9306	2	2	DataLog_HMI2_20250416.csv	2025-04-23 10:33:14.921851
11017	9307	2	2	DataLog_HMI2_20250416.csv	2025-04-23 10:33:14.921851
11018	9308	2	2	DataLog_HMI2_20250416.csv	2025-04-23 10:33:14.921851
11019	9309	2	2	DataLog_HMI2_20250416.csv	2025-04-23 10:33:14.921851
14544	5226	1	01	01_NV4000_202501.CSV	2025-05-22 17:14:16.306798
14545	5227	2	02	02_NV5000_202501.CSV	2025-05-22 17:14:16.384869
14546	5228	2	02	02_NV5000_202501.CSV	2025-05-22 17:14:16.384869
14547	5229	2	02	02_NV5000_202501.CSV	2025-05-22 17:14:16.384869
14548	5230	2	02	02_NV5000_202501.CSV	2025-05-22 17:14:16.384869
14549	5231	2	02	02_NV5000_202501.CSV	2025-05-22 17:14:16.384869
14550	5232	2	02	02_NV5000_202501.CSV	2025-05-22 17:14:16.384869
14551	5233	2	02	02_NV5000_202501.CSV	2025-05-22 17:14:16.384869
14552	5234	2	02	02_NV5000_202501.CSV	2025-05-22 17:14:16.384869
14553	5235	2	02	02_NV5000_202501.CSV	2025-05-22 17:14:16.384869
14554	5236	2	02	02_NV5000_202501.CSV	2025-05-22 17:14:16.384869
14555	5237	2	02	02_NV5000_202501.CSV	2025-05-22 17:14:16.384869
14556	5238	2	02	02_NV5000_202501.CSV	2025-05-22 17:14:16.384869
14557	5239	2	02	02_NV5000_202501.CSV	2025-05-22 17:14:16.384869
14558	5240	2	02	02_NV5000_202501.CSV	2025-05-22 17:14:16.384869
14559	5241	2	02	02_NV5000_202501.CSV	2025-05-22 17:14:16.384869
14560	5242	2	02	02_NV5000_202501.CSV	2025-05-22 17:14:16.384869
14561	5243	2	02	02_NV5000_202501.CSV	2025-05-22 17:14:16.384869
14562	5244	2	02	02_NV5000_202501.CSV	2025-05-22 17:14:16.384869
14563	5245	2	02	02_NV5000_202501.CSV	2025-05-22 17:14:16.384869
14564	5246	2	02	02_NV5000_202501.CSV	2025-05-22 17:14:16.384869
14565	5247	2	02	02_NV5000_202501.CSV	2025-05-22 17:14:16.384869
14566	5248	2	02	02_NV5000_202501.CSV	2025-05-22 17:14:16.384869
14567	5249	2	02	02_NV5000_202501.CSV	2025-05-22 17:14:16.384869
14568	5250	1	09	09_NakamuraTome_SC260-1_202501.CSV	2025-05-22 17:14:16.450568
14569	5251	1	09	09_NakamuraTome_SC260-1_202501.CSV	2025-05-22 17:14:16.450568
14570	5252	1	09	09_NakamuraTome_SC260-1_202501.CSV	2025-05-22 17:14:16.450568
14571	5253	1	09	09_NakamuraTome_SC260-1_202501.CSV	2025-05-22 17:14:16.450568
14572	5254	1	09	09_NakamuraTome_SC260-1_202501.CSV	2025-05-22 17:14:16.450568
14573	5255	1	09	09_NakamuraTome_SC260-1_202501.CSV	2025-05-22 17:14:16.450568
14574	5256	1	09	09_NakamuraTome_SC260-1_202501.CSV	2025-05-22 17:14:16.450568
14575	5257	1	09	09_NakamuraTome_SC260-1_202501.CSV	2025-05-22 17:14:16.450568
14576	5258	1	09	09_NakamuraTome_SC260-1_202501.CSV	2025-05-22 17:14:16.450568
14577	5259	1	09	09_NakamuraTome_SC260-1_202501.CSV	2025-05-22 17:14:16.450568
14578	5260	1	09	09_NakamuraTome_SC260-1_202501.CSV	2025-05-22 17:14:16.450568
14579	5261	1	09	09_NakamuraTome_SC260-1_202501.CSV	2025-05-22 17:14:16.450568
14580	5262	1	09	09_NakamuraTome_SC260-1_202501.CSV	2025-05-22 17:14:16.450568
14581	5263	1	09	09_NakamuraTome_SC260-1_202501.CSV	2025-05-22 17:14:16.450568
14582	5264	1	09	09_NakamuraTome_SC260-1_202501.CSV	2025-05-22 17:14:16.450568
14583	5265	1	09	09_NakamuraTome_SC260-1_202501.CSV	2025-05-22 17:14:16.450568
14584	5266	1	09	09_NakamuraTome_SC260-1_202501.CSV	2025-05-22 17:14:16.450568
14585	5267	1	09	09_NakamuraTome_SC260-1_202501.CSV	2025-05-22 17:14:16.450568
14586	5268	1	09	09_NakamuraTome_SC260-1_202501.CSV	2025-05-22 17:14:16.450568
14587	5269	1	09	09_NakamuraTome_SC260-1_202501.CSV	2025-05-22 17:14:16.450568
14588	5270	1	09	09_NakamuraTome_SC260-1_202501.CSV	2025-05-22 17:14:16.450568
14589	5271	1	09	09_NakamuraTome_SC260-1_202501.CSV	2025-05-22 17:14:16.450568
14590	5272	1	09	09_NakamuraTome_SC260-1_202501.CSV	2025-05-22 17:14:16.450568
11025	3524	2	2	2_NV5000_202503.CSV	2025-04-23 10:41:48.706846
11026	3525	2	2	2_NV5000_202503.CSV	2025-04-23 10:41:48.706846
11027	3526	2	2	2_NV5000_202503.CSV	2025-04-23 10:41:48.706846
11028	3527	2	2	2_NV5000_202503.CSV	2025-04-23 10:41:48.706846
11029	3528	2	2	2_NV5000_202503.CSV	2025-04-23 10:41:48.706846
11030	3529	2	2	2_NV5000_202503.CSV	2025-04-23 10:41:48.706846
11031	3530	2	2	2_NV5000_202503.CSV	2025-04-23 10:41:48.706846
11032	3531	2	2	2_NV5000_202503.CSV	2025-04-23 10:41:48.706846
11033	3532	2	2	2_NV5000_202503.CSV	2025-04-23 10:41:48.706846
11034	3533	2	2	2_NV5000_202503.CSV	2025-04-23 10:41:48.706846
11035	3534	2	2	2_NV5000_202503.CSV	2025-04-23 10:41:48.706846
11036	3535	2	2	2_NV5000_202503.CSV	2025-04-23 10:41:48.706846
11037	3536	2	2	2_NV5000_202503.CSV	2025-04-23 10:41:48.706846
11038	3537	2	2	2_NV5000_202503.CSV	2025-04-23 10:41:48.706846
11039	3538	2	2	2_NV5000_202503.CSV	2025-04-23 10:41:48.706846
11040	3539	2	2	2_NV5000_202503.CSV	2025-04-23 10:41:48.706846
11041	3540	2	2	2_NV5000_202503.CSV	2025-04-23 10:41:48.706846
11042	3541	2	2	2_NV5000_202503.CSV	2025-04-23 10:41:48.706846
11043	3542	2	2	2_NV5000_202503.CSV	2025-04-23 10:41:48.706846
11044	3543	2	2	2_NV5000_202503.CSV	2025-04-23 10:41:48.706846
11045	3544	2	2	2_NV5000_202503.CSV	2025-04-23 10:41:48.706846
14591	5273	1	04	04_Enshu450V_202501.CSV	2025-05-22 17:15:16.395917
14592	5274	1	04	04_Enshu450V_202501.CSV	2025-05-22 17:15:16.395917
14593	5275	1	04	04_Enshu450V_202501.CSV	2025-05-22 17:15:16.395917
14594	5276	1	04	04_Enshu450V_202501.CSV	2025-05-22 17:15:16.395917
14595	5277	1	04	04_Enshu450V_202501.CSV	2025-05-22 17:15:16.395917
14596	5278	1	04	04_Enshu450V_202501.CSV	2025-05-22 17:15:16.395917
14597	5279	1	04	04_Enshu450V_202501.CSV	2025-05-22 17:15:16.395917
14598	5280	1	04	04_Enshu450V_202501.CSV	2025-05-22 17:15:16.395917
14599	5281	1	04	04_Enshu450V_202501.CSV	2025-05-22 17:15:16.395917
14600	5282	1	04	04_Enshu450V_202501.CSV	2025-05-22 17:15:16.395917
14601	5283	1	04	04_Enshu450V_202501.CSV	2025-05-22 17:15:16.395917
14602	5284	1	04	04_Enshu450V_202501.CSV	2025-05-22 17:15:16.395917
14603	5285	1	04	04_Enshu450V_202501.CSV	2025-05-22 17:15:16.395917
14604	5286	1	04	04_Enshu450V_202501.CSV	2025-05-22 17:15:16.395917
14605	5287	1	04	04_Enshu450V_202501.CSV	2025-05-22 17:15:16.395917
14606	5288	1	04	04_Enshu450V_202501.CSV	2025-05-22 17:15:16.395917
14607	5289	1	04	04_Enshu450V_202501.CSV	2025-05-22 17:15:16.395917
14608	5290	1	04	04_Enshu450V_202501.CSV	2025-05-22 17:15:16.395917
14609	5291	1	04	04_Enshu450V_202501.CSV	2025-05-22 17:15:16.395917
14610	5292	1	04	04_Enshu450V_202501.CSV	2025-05-22 17:15:16.395917
14611	5293	1	04	04_Enshu450V_202501.CSV	2025-05-22 17:15:16.395917
14612	5294	1	04	04_Enshu450V_202501.CSV	2025-05-22 17:15:16.395917
14613	5295	1	04	04_Enshu450V_202501.CSV	2025-05-22 17:15:16.395917
11046	9315	1	1	DataLog_HMI1_20250303.csv	2025-04-23 10:49:59.187851
11047	9316	1	1	DataLog_HMI1_20250303.csv	2025-04-23 10:49:59.187851
11048	9317	1	1	DataLog_HMI1_20250303.csv	2025-04-23 10:49:59.187851
11049	9318	1	1	DataLog_HMI1_20250303.csv	2025-04-23 10:49:59.187851
11050	9319	1	1	DataLog_HMI1_20250303.csv	2025-04-23 10:49:59.187851
11051	9320	1	1	DataLog_HMI1_20250303.csv	2025-04-23 10:49:59.187851
11052	9321	1	1	DataLog_HMI1_20250303.csv	2025-04-23 10:49:59.187851
11053	9322	1	1	DataLog_HMI1_20250303.csv	2025-04-23 10:49:59.187851
11054	9323	1	1	DataLog_HMI1_20250303.csv	2025-04-23 10:49:59.187851
11055	9324	1	1	DataLog_HMI1_20250303.csv	2025-04-23 10:49:59.187851
11056	9325	1	1	DataLog_HMI1_20250303.csv	2025-04-23 10:49:59.187851
11057	9326	1	1	DataLog_HMI1_20250303.csv	2025-04-23 10:49:59.187851
11058	3545	1	1	1_NV4000_202503.CSV	2025-04-24 11:50:37.297063
11059	3546	1	1	1_NV4000_202503.CSV	2025-04-24 11:50:37.297063
11060	3547	1	1	1_NV4000_202503.CSV	2025-04-24 11:50:37.297063
11061	3548	1	1	1_NV4000_202503.CSV	2025-04-24 11:50:37.297063
11062	3549	1	1	1_NV4000_202503.CSV	2025-04-24 11:50:37.297063
11063	3550	1	1	1_NV4000_202503.CSV	2025-04-24 11:50:37.297063
11064	3551	1	1	1_NV4000_202503.CSV	2025-04-24 11:50:37.297063
11065	3552	1	1	1_NV4000_202503.CSV	2025-04-24 11:50:37.297063
11066	3553	1	1	1_NV4000_202503.CSV	2025-04-24 11:50:37.297063
11067	3554	1	1	1_NV4000_202503.CSV	2025-04-24 11:50:37.297063
11068	3555	1	1	1_NV4000_202503.CSV	2025-04-24 11:50:37.297063
11069	3556	1	1	1_NV4000_202503.CSV	2025-04-24 11:50:37.297063
11070	3557	1	1	1_NV4000_202503.CSV	2025-04-24 11:50:37.297063
11071	3558	1	1	1_NV4000_202503.CSV	2025-04-24 11:50:37.297063
11072	3559	1	1	1_NV4000_202503.CSV	2025-04-24 11:50:37.297063
11073	3560	1	1	1_NV4000_202503.CSV	2025-04-24 11:50:37.297063
11074	3561	1	1	1_NV4000_202503.CSV	2025-04-24 11:50:37.297063
11075	3562	1	1	1_NV4000_202503.CSV	2025-04-24 11:50:37.297063
11076	3563	1	1	1_NV4000_202503.CSV	2025-04-24 11:50:37.297063
11077	3564	1	1	1_NV4000_202503.CSV	2025-04-24 11:50:37.297063
11078	3565	1	1	1_NV4000_202503.CSV	2025-04-24 11:50:37.297063
11079	3566	1	1	1_NV4000_202503.CSV	2025-04-24 11:50:37.297063
11080	3567	2	2	2_NV5000_202503.CSV	2025-04-24 17:59:56.656425
11081	3568	2	2	2_NV5000_202503.CSV	2025-04-24 17:59:56.656425
11082	3569	2	2	2_NV5000_202503.CSV	2025-04-24 17:59:56.656425
11083	3570	2	2	2_NV5000_202503.CSV	2025-04-24 17:59:56.656425
11084	3571	2	2	2_NV5000_202503.CSV	2025-04-24 17:59:56.656425
11085	3572	2	2	2_NV5000_202503.CSV	2025-04-24 17:59:56.656425
11086	3573	2	2	2_NV5000_202503.CSV	2025-04-24 17:59:56.656425
11087	3574	2	2	2_NV5000_202503.CSV	2025-04-24 17:59:56.656425
11088	3575	2	2	2_NV5000_202503.CSV	2025-04-24 17:59:56.656425
11089	3576	2	2	2_NV5000_202503.CSV	2025-04-24 17:59:56.656425
11090	3577	2	2	2_NV5000_202503.CSV	2025-04-24 17:59:56.656425
11091	3578	2	2	2_NV5000_202503.CSV	2025-04-24 17:59:56.656425
11092	3579	2	2	2_NV5000_202503.CSV	2025-04-24 17:59:56.656425
11093	3580	2	2	2_NV5000_202503.CSV	2025-04-24 17:59:56.656425
11094	3581	2	2	2_NV5000_202503.CSV	2025-04-24 17:59:56.656425
11095	3582	2	2	2_NV5000_202503.CSV	2025-04-24 17:59:56.656425
11096	3583	2	2	2_NV5000_202503.CSV	2025-04-24 17:59:56.656425
11097	3584	2	2	2_NV5000_202503.CSV	2025-04-24 17:59:56.656425
11098	3585	2	2	2_NV5000_202503.CSV	2025-04-24 17:59:56.656425
11099	3586	2	2	2_NV5000_202503.CSV	2025-04-24 17:59:56.656425
11100	3587	2	2	2_NV5000_202503.CSV	2025-04-24 17:59:56.656425
11101	4272	2	19	19_NC-Rooter1_202410.CSV	2025-04-28 10:11:27.694135
11102	4273	2	19	19_NC-Rooter1_202410.CSV	2025-04-28 10:11:27.694135
11103	4274	2	19	19_NC-Rooter1_202410.CSV	2025-04-28 10:11:27.694135
11104	4275	2	19	19_NC-Rooter1_202410.CSV	2025-04-28 10:11:27.694135
11105	4276	2	19	19_NC-Rooter1_202410.CSV	2025-04-28 10:11:27.694135
11106	4277	2	19	19_NC-Rooter1_202410.CSV	2025-04-28 10:11:27.694135
11107	4278	2	19	19_NC-Rooter1_202410.CSV	2025-04-28 10:11:27.694135
11108	4279	2	19	19_NC-Rooter1_202410.CSV	2025-04-28 10:11:27.694135
11109	4280	2	19	19_NC-Rooter1_202410.CSV	2025-04-28 10:11:27.694135
11110	4281	2	19	19_NC-Rooter1_202410.CSV	2025-04-28 10:11:27.694135
11111	4282	2	19	19_NC-Rooter1_202410.CSV	2025-04-28 10:11:27.694135
11112	4283	2	19	19_NC-Rooter1_202410.CSV	2025-04-28 10:11:27.694135
11113	4284	2	19	19_NC-Rooter1_202410.CSV	2025-04-28 10:11:27.694135
11114	4285	2	19	19_NC-Rooter1_202410.CSV	2025-04-28 10:11:27.694135
11115	4286	2	19	19_NC-Rooter1_202410.CSV	2025-04-28 10:11:27.694135
11116	4287	2	19	19_NC-Rooter1_202410.CSV	2025-04-28 10:11:27.694135
11117	4288	2	19	19_NC-Rooter1_202410.CSV	2025-04-28 10:11:27.694135
11118	4289	2	19	19_NC-Rooter1_202410.CSV	2025-04-28 10:11:27.694135
11119	4290	2	19	19_NC-Rooter1_202410.CSV	2025-04-28 10:11:27.694135
11120	4291	2	19	19_NC-Rooter1_202410.CSV	2025-04-28 10:11:27.694135
11121	4292	2	19	19_NC-Rooter1_202410.CSV	2025-04-28 10:11:27.694135
11122	4293	2	19	19_NC-Rooter1_202411.CSV	2025-04-28 10:11:43.350282
11123	4294	2	19	19_NC-Rooter1_202411.CSV	2025-04-28 10:11:43.350282
11124	4295	2	19	19_NC-Rooter1_202411.CSV	2025-04-28 10:11:43.350282
11125	4296	2	19	19_NC-Rooter1_202411.CSV	2025-04-28 10:11:43.350282
11126	4297	2	19	19_NC-Rooter1_202411.CSV	2025-04-28 10:11:43.350282
11127	4298	2	19	19_NC-Rooter1_202411.CSV	2025-04-28 10:11:43.350282
11128	4299	2	19	19_NC-Rooter1_202411.CSV	2025-04-28 10:11:43.350282
11129	4300	2	19	19_NC-Rooter1_202411.CSV	2025-04-28 10:11:43.350282
11130	4301	2	19	19_NC-Rooter1_202411.CSV	2025-04-28 10:11:43.350282
11131	4302	2	19	19_NC-Rooter1_202411.CSV	2025-04-28 10:11:43.350282
11132	4303	2	19	19_NC-Rooter1_202411.CSV	2025-04-28 10:11:43.350282
11133	4304	2	19	19_NC-Rooter1_202411.CSV	2025-04-28 10:11:43.350282
11134	4305	2	19	19_NC-Rooter1_202411.CSV	2025-04-28 10:11:43.350282
11135	4306	2	19	19_NC-Rooter1_202411.CSV	2025-04-28 10:11:43.350282
11136	4307	2	19	19_NC-Rooter1_202411.CSV	2025-04-28 10:11:43.350282
11137	4308	2	19	19_NC-Rooter1_202411.CSV	2025-04-28 10:11:43.350282
11138	4309	2	19	19_NC-Rooter1_202411.CSV	2025-04-28 10:11:43.350282
11139	4310	2	19	19_NC-Rooter1_202411.CSV	2025-04-28 10:11:43.350282
11140	4311	1	1	1_NV4000_202501.CSV	2025-04-28 16:45:20.429301
11141	4312	1	1	1_NV4000_202501.CSV	2025-04-28 16:45:20.429301
11142	4313	1	1	1_NV4000_202501.CSV	2025-04-28 16:45:20.429301
11143	4314	1	1	1_NV4000_202501.CSV	2025-04-28 16:45:20.429301
11144	4315	1	1	1_NV4000_202501.CSV	2025-04-28 16:45:20.429301
11145	4316	1	1	1_NV4000_202501.CSV	2025-04-28 16:45:20.429301
11146	4317	1	1	1_NV4000_202501.CSV	2025-04-28 16:45:20.429301
11147	4318	1	1	1_NV4000_202501.CSV	2025-04-28 16:45:20.429301
11148	4319	1	1	1_NV4000_202501.CSV	2025-04-28 16:45:20.429301
11149	4320	1	1	1_NV4000_202501.CSV	2025-04-28 16:45:20.429301
11150	4321	1	1	1_NV4000_202501.CSV	2025-04-28 16:45:20.429301
11151	4322	1	1	1_NV4000_202501.CSV	2025-04-28 16:45:20.429301
11152	4323	1	1	1_NV4000_202501.CSV	2025-04-28 16:45:20.429301
11153	4324	1	1	1_NV4000_202501.CSV	2025-04-28 16:45:20.429301
11154	4325	1	1	1_NV4000_202501.CSV	2025-04-28 16:45:20.429301
11155	4326	1	1	1_NV4000_202501.CSV	2025-04-28 16:45:20.429301
11156	4327	1	1	1_NV4000_202501.CSV	2025-04-28 16:45:20.429301
11157	4328	1	1	1_NV4000_202501.CSV	2025-04-28 16:45:20.429301
11158	4329	1	1	1_NV4000_202501.CSV	2025-04-28 16:45:20.429301
11159	4330	1	1	1_NV4000_202501.CSV	2025-04-28 16:45:20.429301
11160	4331	1	1	1_NV4000_202501.CSV	2025-04-28 16:45:20.429301
11161	4332	1	1	1_NV4000_202501.CSV	2025-04-28 16:45:20.429301
11162	4333	1	1	1_NV4000_202501.CSV	2025-04-28 16:45:20.429301
11163	4334	1	1	1_NV4000_202501.CSV	2025-04-28 16:48:58.650583
11164	4335	1	1	1_NV4000_202501.CSV	2025-04-28 16:48:58.650583
11165	4336	1	1	1_NV4000_202501.CSV	2025-04-28 16:48:58.650583
11166	4337	1	1	1_NV4000_202501.CSV	2025-04-28 16:48:58.650583
11167	4338	1	1	1_NV4000_202501.CSV	2025-04-28 16:48:58.650583
11168	4339	1	1	1_NV4000_202501.CSV	2025-04-28 16:48:58.650583
11169	4340	1	1	1_NV4000_202501.CSV	2025-04-28 16:48:58.650583
11170	4341	1	1	1_NV4000_202501.CSV	2025-04-28 16:48:58.650583
11171	4342	1	1	1_NV4000_202501.CSV	2025-04-28 16:48:58.650583
11172	4343	1	1	1_NV4000_202501.CSV	2025-04-28 16:48:58.650583
11173	4344	1	1	1_NV4000_202501.CSV	2025-04-28 16:48:58.650583
11174	4345	1	1	1_NV4000_202501.CSV	2025-04-28 16:48:58.650583
11175	4346	1	1	1_NV4000_202501.CSV	2025-04-28 16:48:58.650583
11176	4347	1	1	1_NV4000_202501.CSV	2025-04-28 16:48:58.650583
11177	4348	1	1	1_NV4000_202501.CSV	2025-04-28 16:48:58.650583
11178	4349	1	1	1_NV4000_202501.CSV	2025-04-28 16:48:58.650583
11179	4350	1	1	1_NV4000_202501.CSV	2025-04-28 16:48:58.650583
11180	4351	1	1	1_NV4000_202501.CSV	2025-04-28 16:48:58.650583
11181	4352	1	1	1_NV4000_202501.CSV	2025-04-28 16:48:58.650583
11182	4353	1	1	1_NV4000_202501.CSV	2025-04-28 16:48:58.650583
11183	4354	1	1	1_NV4000_202501.CSV	2025-04-28 16:48:58.650583
11184	4355	1	1	1_NV4000_202501.CSV	2025-04-28 16:48:58.650583
11185	4356	1	1	1_NV4000_202501.CSV	2025-04-28 16:48:58.650583
11186	4357	2	2	2_NV5000_202501.CSV	2025-04-28 17:26:24.303407
11187	4358	2	2	2_NV5000_202501.CSV	2025-04-28 17:26:24.303407
11188	4359	2	2	2_NV5000_202501.CSV	2025-04-28 17:26:24.303407
11189	4360	2	2	2_NV5000_202501.CSV	2025-04-28 17:26:24.303407
11190	4361	2	2	2_NV5000_202501.CSV	2025-04-28 17:26:24.303407
11191	4362	2	2	2_NV5000_202501.CSV	2025-04-28 17:26:24.303407
11192	4363	2	2	2_NV5000_202501.CSV	2025-04-28 17:26:24.303407
11193	4364	2	2	2_NV5000_202501.CSV	2025-04-28 17:26:24.303407
11194	4365	2	2	2_NV5000_202501.CSV	2025-04-28 17:26:24.303407
11195	4366	2	2	2_NV5000_202501.CSV	2025-04-28 17:26:24.303407
11196	4367	2	2	2_NV5000_202501.CSV	2025-04-28 17:26:24.303407
11197	4368	2	2	2_NV5000_202501.CSV	2025-04-28 17:26:24.303407
11198	4369	2	2	2_NV5000_202501.CSV	2025-04-28 17:26:24.303407
11199	4370	2	2	2_NV5000_202501.CSV	2025-04-28 17:26:24.303407
11200	4371	2	2	2_NV5000_202501.CSV	2025-04-28 17:26:24.303407
11201	4372	2	2	2_NV5000_202501.CSV	2025-04-28 17:26:24.303407
11202	4373	2	2	2_NV5000_202501.CSV	2025-04-28 17:26:24.303407
11203	4374	2	2	2_NV5000_202501.CSV	2025-04-28 17:26:24.303407
11204	4375	2	2	2_NV5000_202501.CSV	2025-04-28 17:26:24.303407
11205	4376	2	2	2_NV5000_202501.CSV	2025-04-28 17:26:24.303407
11206	4377	2	2	2_NV5000_202501.CSV	2025-04-28 17:26:24.303407
11207	4378	2	2	2_NV5000_202501.CSV	2025-04-28 17:26:24.303407
11208	4379	2	2	2_NV5000_202501.CSV	2025-04-28 17:26:24.303407
11209	4380	1	3	3_Enshu450FV_202501.CSV	2025-04-28 17:46:28.38795
11210	4381	1	3	3_Enshu450FV_202501.CSV	2025-04-28 17:46:28.38795
11211	4382	1	3	3_Enshu450FV_202501.CSV	2025-04-28 17:46:28.38795
11212	4383	1	3	3_Enshu450FV_202501.CSV	2025-04-28 17:46:28.38795
11213	4384	1	3	3_Enshu450FV_202501.CSV	2025-04-28 17:46:28.38795
11214	4385	1	3	3_Enshu450FV_202501.CSV	2025-04-28 17:46:28.38795
11215	4386	1	3	3_Enshu450FV_202501.CSV	2025-04-28 17:46:28.38795
11216	4387	1	3	3_Enshu450FV_202501.CSV	2025-04-28 17:46:28.38795
11217	4388	1	3	3_Enshu450FV_202501.CSV	2025-04-28 17:46:28.38795
11218	4389	1	3	3_Enshu450FV_202501.CSV	2025-04-28 17:46:28.38795
11219	4390	1	3	3_Enshu450FV_202501.CSV	2025-04-28 17:46:28.38795
11220	4391	1	3	3_Enshu450FV_202501.CSV	2025-04-28 17:46:28.38795
11221	4392	1	3	3_Enshu450FV_202501.CSV	2025-04-28 17:46:28.38795
11222	4393	1	3	3_Enshu450FV_202501.CSV	2025-04-28 17:46:28.38795
11223	4394	1	3	3_Enshu450FV_202501.CSV	2025-04-28 17:46:28.38795
11224	4395	1	3	3_Enshu450FV_202501.CSV	2025-04-28 17:46:28.38795
11225	4396	1	3	3_Enshu450FV_202501.CSV	2025-04-28 17:46:28.38795
11226	4397	1	3	3_Enshu450FV_202501.CSV	2025-04-28 17:46:28.38795
11227	4398	1	3	3_Enshu450FV_202501.CSV	2025-04-28 17:46:28.38795
11228	4399	1	3	3_Enshu450FV_202501.CSV	2025-04-28 17:46:28.38795
11229	4400	1	3	3_Enshu450FV_202501.CSV	2025-04-28 17:46:28.38795
11230	4401	1	3	3_Enshu450FV_202501.CSV	2025-04-28 17:46:28.38795
11231	4402	1	3	3_Enshu450FV_202501.CSV	2025-04-28 17:46:28.38795
11232	4403	1	1	1_NV4000_202501.CSV	2025-04-29 11:35:26.646709
11233	4404	1	1	1_NV4000_202501.CSV	2025-04-29 11:35:26.646709
11234	4405	1	1	1_NV4000_202501.CSV	2025-04-29 11:35:26.646709
11235	4406	1	1	1_NV4000_202501.CSV	2025-04-29 11:35:26.646709
11236	4407	1	1	1_NV4000_202501.CSV	2025-04-29 11:35:26.646709
11237	4408	1	1	1_NV4000_202501.CSV	2025-04-29 11:35:26.646709
11238	4409	1	1	1_NV4000_202501.CSV	2025-04-29 11:35:26.646709
11239	4410	1	1	1_NV4000_202501.CSV	2025-04-29 11:35:26.646709
11240	4411	1	1	1_NV4000_202501.CSV	2025-04-29 11:35:26.646709
11241	4412	1	1	1_NV4000_202501.CSV	2025-04-29 11:35:26.646709
11242	4413	1	1	1_NV4000_202501.CSV	2025-04-29 11:35:26.646709
11243	4414	1	1	1_NV4000_202501.CSV	2025-04-29 11:35:26.646709
11244	4415	1	1	1_NV4000_202501.CSV	2025-04-29 11:35:26.646709
11245	4416	1	1	1_NV4000_202501.CSV	2025-04-29 11:35:26.646709
11246	4417	1	1	1_NV4000_202501.CSV	2025-04-29 11:35:26.646709
11247	4418	1	1	1_NV4000_202501.CSV	2025-04-29 11:35:26.646709
11248	4419	1	1	1_NV4000_202501.CSV	2025-04-29 11:35:26.646709
11249	4420	1	1	1_NV4000_202501.CSV	2025-04-29 11:35:26.646709
11250	4421	1	1	1_NV4000_202501.CSV	2025-04-29 11:35:26.646709
11251	4422	1	1	1_NV4000_202501.CSV	2025-04-29 11:35:26.646709
11252	4423	1	1	1_NV4000_202501.CSV	2025-04-29 11:35:26.646709
11253	4424	1	1	1_NV4000_202501.CSV	2025-04-29 11:35:26.646709
11254	4425	1	1	1_NV4000_202501.CSV	2025-04-29 11:35:26.646709
11255	4426	2	2	2_NV5000_202501.CSV	2025-04-29 11:35:46.638346
11256	4427	2	2	2_NV5000_202501.CSV	2025-04-29 11:35:46.638346
11257	4428	2	2	2_NV5000_202501.CSV	2025-04-29 11:35:46.638346
11258	4429	2	2	2_NV5000_202501.CSV	2025-04-29 11:35:46.638346
11259	4430	2	2	2_NV5000_202501.CSV	2025-04-29 11:35:46.638346
11260	4431	2	2	2_NV5000_202501.CSV	2025-04-29 11:35:46.638346
11261	4432	2	2	2_NV5000_202501.CSV	2025-04-29 11:35:46.638346
11262	4433	2	2	2_NV5000_202501.CSV	2025-04-29 11:35:46.638346
11263	4434	2	2	2_NV5000_202501.CSV	2025-04-29 11:35:46.638346
11264	4435	2	2	2_NV5000_202501.CSV	2025-04-29 11:35:46.638346
11265	4436	2	2	2_NV5000_202501.CSV	2025-04-29 11:35:46.638346
11266	4437	2	2	2_NV5000_202501.CSV	2025-04-29 11:35:46.638346
11267	4438	2	2	2_NV5000_202501.CSV	2025-04-29 11:35:46.638346
11268	4439	2	2	2_NV5000_202501.CSV	2025-04-29 11:35:46.638346
11269	4440	2	2	2_NV5000_202501.CSV	2025-04-29 11:35:46.638346
11270	4441	2	2	2_NV5000_202501.CSV	2025-04-29 11:35:46.638346
11271	4442	2	2	2_NV5000_202501.CSV	2025-04-29 11:35:46.638346
11272	4443	2	2	2_NV5000_202501.CSV	2025-04-29 11:35:46.638346
11273	4444	2	2	2_NV5000_202501.CSV	2025-04-29 11:35:46.638346
11274	4445	2	2	2_NV5000_202501.CSV	2025-04-29 11:35:46.638346
11275	4446	2	2	2_NV5000_202501.CSV	2025-04-29 11:35:46.638346
11276	4447	2	2	2_NV5000_202501.CSV	2025-04-29 11:35:46.638346
11277	4448	2	2	2_NV5000_202501.CSV	2025-04-29 11:35:46.638346
11278	4449	1	3	3_Enshu450FV_202501.CSV	2025-04-29 11:35:56.714247
11279	4450	1	3	3_Enshu450FV_202501.CSV	2025-04-29 11:35:56.714247
11280	4451	1	3	3_Enshu450FV_202501.CSV	2025-04-29 11:35:56.714247
11281	4452	1	3	3_Enshu450FV_202501.CSV	2025-04-29 11:35:56.714247
11282	4453	1	3	3_Enshu450FV_202501.CSV	2025-04-29 11:35:56.714247
11283	4454	1	3	3_Enshu450FV_202501.CSV	2025-04-29 11:35:56.714247
11284	4455	1	3	3_Enshu450FV_202501.CSV	2025-04-29 11:35:56.714247
11285	4456	1	3	3_Enshu450FV_202501.CSV	2025-04-29 11:35:56.714247
11286	4457	1	3	3_Enshu450FV_202501.CSV	2025-04-29 11:35:56.714247
11287	4458	1	3	3_Enshu450FV_202501.CSV	2025-04-29 11:35:56.714247
11288	4459	1	3	3_Enshu450FV_202501.CSV	2025-04-29 11:35:56.714247
11289	4460	1	3	3_Enshu450FV_202501.CSV	2025-04-29 11:35:56.714247
11290	4461	1	3	3_Enshu450FV_202501.CSV	2025-04-29 11:35:56.714247
11291	4462	1	3	3_Enshu450FV_202501.CSV	2025-04-29 11:35:56.714247
11292	4463	1	3	3_Enshu450FV_202501.CSV	2025-04-29 11:35:56.714247
11293	4464	1	3	3_Enshu450FV_202501.CSV	2025-04-29 11:35:56.714247
11294	4465	1	3	3_Enshu450FV_202501.CSV	2025-04-29 11:35:56.714247
11295	4466	1	3	3_Enshu450FV_202501.CSV	2025-04-29 11:35:56.714247
11296	4467	1	3	3_Enshu450FV_202501.CSV	2025-04-29 11:35:56.714247
11297	4468	1	3	3_Enshu450FV_202501.CSV	2025-04-29 11:35:56.714247
11298	4469	1	3	3_Enshu450FV_202501.CSV	2025-04-29 11:35:56.714247
11299	4470	1	3	3_Enshu450FV_202501.CSV	2025-04-29 11:35:56.714247
11300	4471	1	3	3_Enshu450FV_202501.CSV	2025-04-29 11:35:56.714247
11301	4472	1	4	4_Enshu450V_202501.CSV	2025-04-29 11:36:14.877326
11302	4473	1	4	4_Enshu450V_202501.CSV	2025-04-29 11:36:14.877326
11303	4474	1	4	4_Enshu450V_202501.CSV	2025-04-29 11:36:14.877326
11304	4475	1	4	4_Enshu450V_202501.CSV	2025-04-29 11:36:14.877326
11305	4476	1	4	4_Enshu450V_202501.CSV	2025-04-29 11:36:14.877326
11306	4477	1	4	4_Enshu450V_202501.CSV	2025-04-29 11:36:14.877326
11307	4478	1	4	4_Enshu450V_202501.CSV	2025-04-29 11:36:14.877326
11308	4479	1	4	4_Enshu450V_202501.CSV	2025-04-29 11:36:14.877326
11309	4480	1	4	4_Enshu450V_202501.CSV	2025-04-29 11:36:14.877326
11310	4481	1	4	4_Enshu450V_202501.CSV	2025-04-29 11:36:14.877326
11311	4482	1	4	4_Enshu450V_202501.CSV	2025-04-29 11:36:14.877326
11312	4483	1	4	4_Enshu450V_202501.CSV	2025-04-29 11:36:14.877326
11313	4484	1	4	4_Enshu450V_202501.CSV	2025-04-29 11:36:14.877326
11314	4485	1	4	4_Enshu450V_202501.CSV	2025-04-29 11:36:14.877326
11315	4486	1	4	4_Enshu450V_202501.CSV	2025-04-29 11:36:14.877326
11316	4487	1	4	4_Enshu450V_202501.CSV	2025-04-29 11:36:14.877326
11317	4488	1	4	4_Enshu450V_202501.CSV	2025-04-29 11:36:14.877326
11318	4489	1	4	4_Enshu450V_202501.CSV	2025-04-29 11:36:14.877326
11319	4490	1	4	4_Enshu450V_202501.CSV	2025-04-29 11:36:14.877326
11320	4491	1	4	4_Enshu450V_202501.CSV	2025-04-29 11:36:14.877326
11321	4492	1	4	4_Enshu450V_202501.CSV	2025-04-29 11:36:14.877326
11322	4493	1	4	4_Enshu450V_202501.CSV	2025-04-29 11:36:14.877326
11323	4494	1	4	4_Enshu450V_202501.CSV	2025-04-29 11:36:14.877326
11324	4495	1	5	5_Enshu1_202501.CSV	2025-04-29 11:36:23.429637
11325	4496	1	5	5_Enshu1_202501.CSV	2025-04-29 11:36:23.429637
11326	4497	1	5	5_Enshu1_202501.CSV	2025-04-29 11:36:23.429637
11327	4498	1	5	5_Enshu1_202501.CSV	2025-04-29 11:36:23.429637
11328	4499	1	5	5_Enshu1_202501.CSV	2025-04-29 11:36:23.429637
11329	4500	1	5	5_Enshu1_202501.CSV	2025-04-29 11:36:23.429637
11330	4501	1	5	5_Enshu1_202501.CSV	2025-04-29 11:36:23.429637
11331	4502	1	5	5_Enshu1_202501.CSV	2025-04-29 11:36:23.429637
11332	4503	1	5	5_Enshu1_202501.CSV	2025-04-29 11:36:23.429637
11333	4504	1	5	5_Enshu1_202501.CSV	2025-04-29 11:36:23.429637
11334	4505	1	5	5_Enshu1_202501.CSV	2025-04-29 11:36:23.429637
11335	4506	1	5	5_Enshu1_202501.CSV	2025-04-29 11:36:23.429637
11336	4507	1	5	5_Enshu1_202501.CSV	2025-04-29 11:36:23.429637
11337	4508	1	5	5_Enshu1_202501.CSV	2025-04-29 11:36:23.429637
11338	4509	1	5	5_Enshu1_202501.CSV	2025-04-29 11:36:23.429637
11339	4510	1	5	5_Enshu1_202501.CSV	2025-04-29 11:36:23.429637
11340	4511	1	5	5_Enshu1_202501.CSV	2025-04-29 11:36:23.429637
11341	4512	1	5	5_Enshu1_202501.CSV	2025-04-29 11:36:23.429637
11342	4513	1	5	5_Enshu1_202501.CSV	2025-04-29 11:36:23.429637
11343	4514	1	5	5_Enshu1_202501.CSV	2025-04-29 11:36:23.429637
11344	4515	1	5	5_Enshu1_202501.CSV	2025-04-29 11:36:23.429637
11345	4516	1	5	5_Enshu1_202501.CSV	2025-04-29 11:36:23.429637
11346	4517	1	5	5_Enshu1_202501.CSV	2025-04-29 11:36:23.429637
11347	4518	2	6	6_Enshu2_202501.CSV	2025-04-29 11:36:32.736456
11348	4519	2	6	6_Enshu2_202501.CSV	2025-04-29 11:36:32.736456
11349	4520	2	6	6_Enshu2_202501.CSV	2025-04-29 11:36:32.736456
11350	4521	2	6	6_Enshu2_202501.CSV	2025-04-29 11:36:32.736456
11351	4522	2	6	6_Enshu2_202501.CSV	2025-04-29 11:36:32.736456
11352	4523	2	6	6_Enshu2_202501.CSV	2025-04-29 11:36:32.736456
11353	4524	2	6	6_Enshu2_202501.CSV	2025-04-29 11:36:32.736456
11354	4525	2	6	6_Enshu2_202501.CSV	2025-04-29 11:36:32.736456
11355	4526	2	6	6_Enshu2_202501.CSV	2025-04-29 11:36:32.736456
11356	4527	2	6	6_Enshu2_202501.CSV	2025-04-29 11:36:32.736456
11357	4528	2	6	6_Enshu2_202501.CSV	2025-04-29 11:36:32.736456
11358	4529	2	6	6_Enshu2_202501.CSV	2025-04-29 11:36:32.736456
11359	4530	2	6	6_Enshu2_202501.CSV	2025-04-29 11:36:32.736456
11360	4531	2	6	6_Enshu2_202501.CSV	2025-04-29 11:36:32.736456
11361	4532	2	6	6_Enshu2_202501.CSV	2025-04-29 11:36:32.736456
11362	4533	2	6	6_Enshu2_202501.CSV	2025-04-29 11:36:32.736456
11363	4534	2	6	6_Enshu2_202501.CSV	2025-04-29 11:36:32.736456
11364	4535	2	6	6_Enshu2_202501.CSV	2025-04-29 11:36:32.736456
11365	4536	2	6	6_Enshu2_202501.CSV	2025-04-29 11:36:32.736456
11366	4537	2	6	6_Enshu2_202501.CSV	2025-04-29 11:36:32.736456
11367	4538	2	6	6_Enshu2_202501.CSV	2025-04-29 11:36:32.736456
11368	4539	2	6	6_Enshu2_202501.CSV	2025-04-29 11:36:32.736456
11369	4540	2	6	6_Enshu2_202501.CSV	2025-04-29 11:36:32.736456
11370	4541	1	7	7_Yamazaki-SEV320NCR_202501.CSV	2025-04-29 11:36:41.735191
11371	4542	1	7	7_Yamazaki-SEV320NCR_202501.CSV	2025-04-29 11:36:41.735191
11372	4543	1	7	7_Yamazaki-SEV320NCR_202501.CSV	2025-04-29 11:36:41.735191
11373	4544	1	7	7_Yamazaki-SEV320NCR_202501.CSV	2025-04-29 11:36:41.735191
11374	4545	1	7	7_Yamazaki-SEV320NCR_202501.CSV	2025-04-29 11:36:41.735191
11375	4546	1	7	7_Yamazaki-SEV320NCR_202501.CSV	2025-04-29 11:36:41.735191
11376	4547	1	7	7_Yamazaki-SEV320NCR_202501.CSV	2025-04-29 11:36:41.735191
11377	4548	1	7	7_Yamazaki-SEV320NCR_202501.CSV	2025-04-29 11:36:41.735191
11378	4549	1	7	7_Yamazaki-SEV320NCR_202501.CSV	2025-04-29 11:36:41.735191
11379	4550	1	7	7_Yamazaki-SEV320NCR_202501.CSV	2025-04-29 11:36:41.735191
11380	4551	1	7	7_Yamazaki-SEV320NCR_202501.CSV	2025-04-29 11:36:41.735191
11381	4552	1	7	7_Yamazaki-SEV320NCR_202501.CSV	2025-04-29 11:36:41.735191
11382	4553	1	7	7_Yamazaki-SEV320NCR_202501.CSV	2025-04-29 11:36:41.735191
11383	4554	1	7	7_Yamazaki-SEV320NCR_202501.CSV	2025-04-29 11:36:41.735191
11384	4555	1	7	7_Yamazaki-SEV320NCR_202501.CSV	2025-04-29 11:36:41.735191
11385	4556	1	7	7_Yamazaki-SEV320NCR_202501.CSV	2025-04-29 11:36:41.735191
11386	4557	1	7	7_Yamazaki-SEV320NCR_202501.CSV	2025-04-29 11:36:41.735191
11387	4558	1	7	7_Yamazaki-SEV320NCR_202501.CSV	2025-04-29 11:36:41.735191
11388	4559	1	7	7_Yamazaki-SEV320NCR_202501.CSV	2025-04-29 11:36:41.735191
11389	4560	1	7	7_Yamazaki-SEV320NCR_202501.CSV	2025-04-29 11:36:41.735191
11390	4561	1	7	7_Yamazaki-SEV320NCR_202501.CSV	2025-04-29 11:36:41.735191
11391	4562	1	7	7_Yamazaki-SEV320NCR_202501.CSV	2025-04-29 11:36:41.735191
11392	4563	1	7	7_Yamazaki-SEV320NCR_202501.CSV	2025-04-29 11:36:41.735191
11393	4564	2	8	8_NakamuraTome_TMC-15ll_202501.CSV	2025-04-29 11:36:55.047714
11394	4565	2	8	8_NakamuraTome_TMC-15ll_202501.CSV	2025-04-29 11:36:55.047714
11395	4566	2	8	8_NakamuraTome_TMC-15ll_202501.CSV	2025-04-29 11:36:55.047714
11396	4567	2	8	8_NakamuraTome_TMC-15ll_202501.CSV	2025-04-29 11:36:55.047714
11397	4568	2	8	8_NakamuraTome_TMC-15ll_202501.CSV	2025-04-29 11:36:55.047714
11398	4569	2	8	8_NakamuraTome_TMC-15ll_202501.CSV	2025-04-29 11:36:55.047714
11399	4570	2	8	8_NakamuraTome_TMC-15ll_202501.CSV	2025-04-29 11:36:55.047714
11400	4571	2	8	8_NakamuraTome_TMC-15ll_202501.CSV	2025-04-29 11:36:55.047714
11401	4572	2	8	8_NakamuraTome_TMC-15ll_202501.CSV	2025-04-29 11:36:55.047714
11402	4573	2	8	8_NakamuraTome_TMC-15ll_202501.CSV	2025-04-29 11:36:55.047714
11403	4574	2	8	8_NakamuraTome_TMC-15ll_202501.CSV	2025-04-29 11:36:55.047714
11404	4575	2	8	8_NakamuraTome_TMC-15ll_202501.CSV	2025-04-29 11:36:55.047714
11405	4576	2	8	8_NakamuraTome_TMC-15ll_202501.CSV	2025-04-29 11:36:55.047714
11406	4577	2	8	8_NakamuraTome_TMC-15ll_202501.CSV	2025-04-29 11:36:55.047714
11407	4578	2	8	8_NakamuraTome_TMC-15ll_202501.CSV	2025-04-29 11:36:55.047714
11408	4579	2	8	8_NakamuraTome_TMC-15ll_202501.CSV	2025-04-29 11:36:55.047714
11409	4580	2	8	8_NakamuraTome_TMC-15ll_202501.CSV	2025-04-29 11:36:55.047714
11410	4581	2	8	8_NakamuraTome_TMC-15ll_202501.CSV	2025-04-29 11:36:55.047714
11411	4582	2	8	8_NakamuraTome_TMC-15ll_202501.CSV	2025-04-29 11:36:55.047714
11412	4583	2	8	8_NakamuraTome_TMC-15ll_202501.CSV	2025-04-29 11:36:55.047714
11413	4584	2	8	8_NakamuraTome_TMC-15ll_202501.CSV	2025-04-29 11:36:55.047714
11414	4585	2	8	8_NakamuraTome_TMC-15ll_202501.CSV	2025-04-29 11:36:55.047714
11415	4586	2	8	8_NakamuraTome_TMC-15ll_202501.CSV	2025-04-29 11:36:55.047714
11416	4587	1	9	9_NakamuraTome_SC260-1_202501.CSV	2025-04-29 11:37:05.620033
11417	4588	1	9	9_NakamuraTome_SC260-1_202501.CSV	2025-04-29 11:37:05.620033
11418	4589	1	9	9_NakamuraTome_SC260-1_202501.CSV	2025-04-29 11:37:05.620033
11419	4590	1	9	9_NakamuraTome_SC260-1_202501.CSV	2025-04-29 11:37:05.620033
11420	4591	1	9	9_NakamuraTome_SC260-1_202501.CSV	2025-04-29 11:37:05.620033
11421	4592	1	9	9_NakamuraTome_SC260-1_202501.CSV	2025-04-29 11:37:05.620033
11422	4593	1	9	9_NakamuraTome_SC260-1_202501.CSV	2025-04-29 11:37:05.620033
11423	4594	1	9	9_NakamuraTome_SC260-1_202501.CSV	2025-04-29 11:37:05.620033
11424	4595	1	9	9_NakamuraTome_SC260-1_202501.CSV	2025-04-29 11:37:05.620033
11425	4596	1	9	9_NakamuraTome_SC260-1_202501.CSV	2025-04-29 11:37:05.620033
11426	4597	1	9	9_NakamuraTome_SC260-1_202501.CSV	2025-04-29 11:37:05.620033
11427	4598	1	9	9_NakamuraTome_SC260-1_202501.CSV	2025-04-29 11:37:05.620033
11428	4599	1	9	9_NakamuraTome_SC260-1_202501.CSV	2025-04-29 11:37:05.620033
11429	4600	1	9	9_NakamuraTome_SC260-1_202501.CSV	2025-04-29 11:37:05.620033
11430	4601	1	9	9_NakamuraTome_SC260-1_202501.CSV	2025-04-29 11:37:05.620033
11431	4602	1	9	9_NakamuraTome_SC260-1_202501.CSV	2025-04-29 11:37:05.620033
11432	4603	1	9	9_NakamuraTome_SC260-1_202501.CSV	2025-04-29 11:37:05.620033
11433	4604	1	9	9_NakamuraTome_SC260-1_202501.CSV	2025-04-29 11:37:05.620033
11434	4605	1	9	9_NakamuraTome_SC260-1_202501.CSV	2025-04-29 11:37:05.620033
11435	4606	1	9	9_NakamuraTome_SC260-1_202501.CSV	2025-04-29 11:37:05.620033
11436	4607	1	9	9_NakamuraTome_SC260-1_202501.CSV	2025-04-29 11:37:05.620033
11437	4608	1	9	9_NakamuraTome_SC260-1_202501.CSV	2025-04-29 11:37:05.620033
11438	4609	1	9	9_NakamuraTome_SC260-1_202501.CSV	2025-04-29 11:37:05.620033
11439	4610	1	10	10_NakamuraTome_SC260-2_202501.CSV	2025-04-29 11:37:15.184075
11440	4611	1	10	10_NakamuraTome_SC260-2_202501.CSV	2025-04-29 11:37:15.184075
11441	4612	1	10	10_NakamuraTome_SC260-2_202501.CSV	2025-04-29 11:37:15.184075
11442	4613	1	10	10_NakamuraTome_SC260-2_202501.CSV	2025-04-29 11:37:15.184075
11443	4614	1	10	10_NakamuraTome_SC260-2_202501.CSV	2025-04-29 11:37:15.184075
11444	4615	1	10	10_NakamuraTome_SC260-2_202501.CSV	2025-04-29 11:37:15.184075
11445	4616	1	10	10_NakamuraTome_SC260-2_202501.CSV	2025-04-29 11:37:15.184075
11446	4617	1	10	10_NakamuraTome_SC260-2_202501.CSV	2025-04-29 11:37:15.184075
11447	4618	1	10	10_NakamuraTome_SC260-2_202501.CSV	2025-04-29 11:37:15.184075
11448	4619	1	10	10_NakamuraTome_SC260-2_202501.CSV	2025-04-29 11:37:15.184075
11449	4620	1	10	10_NakamuraTome_SC260-2_202501.CSV	2025-04-29 11:37:15.184075
11450	4621	1	10	10_NakamuraTome_SC260-2_202501.CSV	2025-04-29 11:37:15.184075
11451	4622	1	10	10_NakamuraTome_SC260-2_202501.CSV	2025-04-29 11:37:15.184075
11452	4623	1	10	10_NakamuraTome_SC260-2_202501.CSV	2025-04-29 11:37:15.184075
11453	4624	1	10	10_NakamuraTome_SC260-2_202501.CSV	2025-04-29 11:37:15.184075
11454	4625	1	10	10_NakamuraTome_SC260-2_202501.CSV	2025-04-29 11:37:15.184075
11455	4626	1	10	10_NakamuraTome_SC260-2_202501.CSV	2025-04-29 11:37:15.184075
11456	4627	1	10	10_NakamuraTome_SC260-2_202501.CSV	2025-04-29 11:37:15.184075
11457	4628	1	10	10_NakamuraTome_SC260-2_202501.CSV	2025-04-29 11:37:15.184075
11458	4629	1	10	10_NakamuraTome_SC260-2_202501.CSV	2025-04-29 11:37:15.184075
11459	4630	1	10	10_NakamuraTome_SC260-2_202501.CSV	2025-04-29 11:37:15.184075
11460	4631	1	10	10_NakamuraTome_SC260-2_202501.CSV	2025-04-29 11:37:15.184075
11461	4632	1	10	10_NakamuraTome_SC260-2_202501.CSV	2025-04-29 11:37:15.184075
11462	4633	2	11	11_NakamuraTome_SC250_202501.CSV	2025-04-29 11:37:26.41856
11463	4634	2	11	11_NakamuraTome_SC250_202501.CSV	2025-04-29 11:37:26.41856
11464	4635	2	11	11_NakamuraTome_SC250_202501.CSV	2025-04-29 11:37:26.41856
11465	4636	2	11	11_NakamuraTome_SC250_202501.CSV	2025-04-29 11:37:26.41856
11466	4637	2	11	11_NakamuraTome_SC250_202501.CSV	2025-04-29 11:37:26.41856
11467	4638	2	11	11_NakamuraTome_SC250_202501.CSV	2025-04-29 11:37:26.41856
11468	4639	2	11	11_NakamuraTome_SC250_202501.CSV	2025-04-29 11:37:26.41856
11469	4640	2	11	11_NakamuraTome_SC250_202501.CSV	2025-04-29 11:37:26.41856
11470	4641	2	11	11_NakamuraTome_SC250_202501.CSV	2025-04-29 11:37:26.41856
11471	4642	2	11	11_NakamuraTome_SC250_202501.CSV	2025-04-29 11:37:26.41856
11472	4643	2	11	11_NakamuraTome_SC250_202501.CSV	2025-04-29 11:37:26.41856
11473	4644	2	11	11_NakamuraTome_SC250_202501.CSV	2025-04-29 11:37:26.41856
11474	4645	2	11	11_NakamuraTome_SC250_202501.CSV	2025-04-29 11:37:26.41856
11475	4646	2	11	11_NakamuraTome_SC250_202501.CSV	2025-04-29 11:37:26.41856
11476	4647	2	11	11_NakamuraTome_SC250_202501.CSV	2025-04-29 11:37:26.41856
11477	4648	2	11	11_NakamuraTome_SC250_202501.CSV	2025-04-29 11:37:26.41856
11478	4649	2	11	11_NakamuraTome_SC250_202501.CSV	2025-04-29 11:37:26.41856
11479	4650	2	11	11_NakamuraTome_SC250_202501.CSV	2025-04-29 11:37:26.41856
11480	4651	2	11	11_NakamuraTome_SC250_202501.CSV	2025-04-29 11:37:26.41856
11481	4652	2	11	11_NakamuraTome_SC250_202501.CSV	2025-04-29 11:37:26.41856
11482	4653	2	11	11_NakamuraTome_SC250_202501.CSV	2025-04-29 11:37:26.41856
11483	4654	2	11	11_NakamuraTome_SC250_202501.CSV	2025-04-29 11:37:26.41856
11484	4655	2	11	11_NakamuraTome_SC250_202501.CSV	2025-04-29 11:37:26.41856
11485	4656	1	12	12_Takamatsu_202501.CSV	2025-04-29 11:37:38.129324
11486	4657	1	12	12_Takamatsu_202501.CSV	2025-04-29 11:37:38.129324
11487	4658	1	12	12_Takamatsu_202501.CSV	2025-04-29 11:37:38.129324
11488	4659	1	12	12_Takamatsu_202501.CSV	2025-04-29 11:37:38.129324
11489	4660	1	12	12_Takamatsu_202501.CSV	2025-04-29 11:37:38.129324
11490	4661	1	12	12_Takamatsu_202501.CSV	2025-04-29 11:37:38.129324
11491	4662	1	12	12_Takamatsu_202501.CSV	2025-04-29 11:37:38.129324
11492	4663	1	12	12_Takamatsu_202501.CSV	2025-04-29 11:37:38.129324
11493	4664	1	12	12_Takamatsu_202501.CSV	2025-04-29 11:37:38.129324
11494	4665	1	12	12_Takamatsu_202501.CSV	2025-04-29 11:37:38.129324
11495	4666	1	12	12_Takamatsu_202501.CSV	2025-04-29 11:37:38.129324
11496	4667	1	12	12_Takamatsu_202501.CSV	2025-04-29 11:37:38.129324
11497	4668	1	12	12_Takamatsu_202501.CSV	2025-04-29 11:37:38.129324
11498	4669	1	12	12_Takamatsu_202501.CSV	2025-04-29 11:37:38.129324
11499	4670	1	12	12_Takamatsu_202501.CSV	2025-04-29 11:37:38.129324
11500	4671	1	12	12_Takamatsu_202501.CSV	2025-04-29 11:37:38.129324
11501	4672	1	12	12_Takamatsu_202501.CSV	2025-04-29 11:37:38.129324
11502	4673	1	12	12_Takamatsu_202501.CSV	2025-04-29 11:37:38.129324
11503	4674	1	12	12_Takamatsu_202501.CSV	2025-04-29 11:37:38.129324
11504	4675	1	12	12_Takamatsu_202501.CSV	2025-04-29 11:37:38.129324
11505	4676	1	12	12_Takamatsu_202501.CSV	2025-04-29 11:37:38.129324
11506	4677	1	12	12_Takamatsu_202501.CSV	2025-04-29 11:37:38.129324
11507	4678	1	12	12_Takamatsu_202501.CSV	2025-04-29 11:37:38.129324
11508	4679	1	13	13_RoboDrill1_202501.CSV	2025-04-29 11:37:48.987225
11509	4680	1	13	13_RoboDrill1_202501.CSV	2025-04-29 11:37:48.987225
11510	4681	1	13	13_RoboDrill1_202501.CSV	2025-04-29 11:37:48.987225
11511	4682	1	13	13_RoboDrill1_202501.CSV	2025-04-29 11:37:48.987225
11512	4683	1	13	13_RoboDrill1_202501.CSV	2025-04-29 11:37:48.987225
11513	4684	1	13	13_RoboDrill1_202501.CSV	2025-04-29 11:37:48.987225
11514	4685	1	13	13_RoboDrill1_202501.CSV	2025-04-29 11:37:48.987225
11515	4686	1	13	13_RoboDrill1_202501.CSV	2025-04-29 11:37:48.987225
11516	4687	1	13	13_RoboDrill1_202501.CSV	2025-04-29 11:37:48.987225
11517	4688	1	13	13_RoboDrill1_202501.CSV	2025-04-29 11:37:48.987225
11518	4689	1	13	13_RoboDrill1_202501.CSV	2025-04-29 11:37:48.987225
11519	4690	1	13	13_RoboDrill1_202501.CSV	2025-04-29 11:37:48.987225
11520	4691	1	13	13_RoboDrill1_202501.CSV	2025-04-29 11:37:48.987225
11521	4692	1	13	13_RoboDrill1_202501.CSV	2025-04-29 11:37:48.987225
11522	4693	1	13	13_RoboDrill1_202501.CSV	2025-04-29 11:37:48.987225
11523	4694	1	13	13_RoboDrill1_202501.CSV	2025-04-29 11:37:48.987225
11524	4695	1	13	13_RoboDrill1_202501.CSV	2025-04-29 11:37:48.987225
11525	4696	1	13	13_RoboDrill1_202501.CSV	2025-04-29 11:37:48.987225
11526	4697	1	13	13_RoboDrill1_202501.CSV	2025-04-29 11:37:48.987225
11527	4698	1	13	13_RoboDrill1_202501.CSV	2025-04-29 11:37:48.987225
11528	4699	1	13	13_RoboDrill1_202501.CSV	2025-04-29 11:37:48.987225
11529	4700	1	13	13_RoboDrill1_202501.CSV	2025-04-29 11:37:48.987225
11530	4701	1	13	13_RoboDrill1_202501.CSV	2025-04-29 11:37:48.987225
11531	4702	1	14	14_RoboDrill2_202501.CSV	2025-04-29 11:37:59.203864
11532	4703	1	14	14_RoboDrill2_202501.CSV	2025-04-29 11:37:59.203864
11533	4704	1	14	14_RoboDrill2_202501.CSV	2025-04-29 11:37:59.203864
11534	4705	1	14	14_RoboDrill2_202501.CSV	2025-04-29 11:37:59.203864
11535	4706	1	14	14_RoboDrill2_202501.CSV	2025-04-29 11:37:59.203864
11536	4707	1	14	14_RoboDrill2_202501.CSV	2025-04-29 11:37:59.203864
11537	4708	1	14	14_RoboDrill2_202501.CSV	2025-04-29 11:37:59.203864
11538	4709	1	14	14_RoboDrill2_202501.CSV	2025-04-29 11:37:59.203864
11539	4710	1	14	14_RoboDrill2_202501.CSV	2025-04-29 11:37:59.203864
11540	4711	1	14	14_RoboDrill2_202501.CSV	2025-04-29 11:37:59.203864
11541	4712	1	14	14_RoboDrill2_202501.CSV	2025-04-29 11:37:59.203864
11542	4713	1	14	14_RoboDrill2_202501.CSV	2025-04-29 11:37:59.203864
11543	4714	1	14	14_RoboDrill2_202501.CSV	2025-04-29 11:37:59.203864
11544	4715	1	14	14_RoboDrill2_202501.CSV	2025-04-29 11:37:59.203864
11545	4716	1	14	14_RoboDrill2_202501.CSV	2025-04-29 11:37:59.203864
11546	4717	1	14	14_RoboDrill2_202501.CSV	2025-04-29 11:37:59.203864
11547	4718	1	14	14_RoboDrill2_202501.CSV	2025-04-29 11:37:59.203864
11548	4719	1	14	14_RoboDrill2_202501.CSV	2025-04-29 11:37:59.203864
11549	4720	1	14	14_RoboDrill2_202501.CSV	2025-04-29 11:37:59.203864
11550	4721	1	14	14_RoboDrill2_202501.CSV	2025-04-29 11:37:59.203864
11551	4722	1	14	14_RoboDrill2_202501.CSV	2025-04-29 11:37:59.203864
11552	4723	1	14	14_RoboDrill2_202501.CSV	2025-04-29 11:37:59.203864
11553	4724	1	14	14_RoboDrill2_202501.CSV	2025-04-29 11:37:59.203864
11554	4725	1	15	15_RoboDrill3_202501.CSV	2025-04-29 11:38:09.424373
11555	4726	1	15	15_RoboDrill3_202501.CSV	2025-04-29 11:38:09.424373
11556	4727	1	15	15_RoboDrill3_202501.CSV	2025-04-29 11:38:09.424373
11557	4728	1	15	15_RoboDrill3_202501.CSV	2025-04-29 11:38:09.424373
11558	4729	1	15	15_RoboDrill3_202501.CSV	2025-04-29 11:38:09.424373
11559	4730	1	15	15_RoboDrill3_202501.CSV	2025-04-29 11:38:09.424373
11560	4731	1	15	15_RoboDrill3_202501.CSV	2025-04-29 11:38:09.424373
11561	4732	1	15	15_RoboDrill3_202501.CSV	2025-04-29 11:38:09.424373
11562	4733	1	15	15_RoboDrill3_202501.CSV	2025-04-29 11:38:09.424373
11563	4734	1	15	15_RoboDrill3_202501.CSV	2025-04-29 11:38:09.424373
11564	4735	1	15	15_RoboDrill3_202501.CSV	2025-04-29 11:38:09.424373
11565	4736	1	15	15_RoboDrill3_202501.CSV	2025-04-29 11:38:09.424373
11566	4737	1	15	15_RoboDrill3_202501.CSV	2025-04-29 11:38:09.424373
11567	4738	1	15	15_RoboDrill3_202501.CSV	2025-04-29 11:38:09.424373
11568	4739	1	15	15_RoboDrill3_202501.CSV	2025-04-29 11:38:09.424373
11569	4740	1	15	15_RoboDrill3_202501.CSV	2025-04-29 11:38:09.424373
11570	4741	1	15	15_RoboDrill3_202501.CSV	2025-04-29 11:38:09.424373
11571	4742	1	15	15_RoboDrill3_202501.CSV	2025-04-29 11:38:09.424373
11572	4743	1	15	15_RoboDrill3_202501.CSV	2025-04-29 11:38:09.424373
11573	4744	1	15	15_RoboDrill3_202501.CSV	2025-04-29 11:38:09.424373
11574	4745	1	15	15_RoboDrill3_202501.CSV	2025-04-29 11:38:09.424373
11575	4746	1	15	15_RoboDrill3_202501.CSV	2025-04-29 11:38:09.424373
11576	4747	1	15	15_RoboDrill3_202501.CSV	2025-04-29 11:38:09.424373
11577	4748	1	16	16_RoboDrill4_202501.CSV	2025-04-29 11:38:24.946914
11578	4749	1	16	16_RoboDrill4_202501.CSV	2025-04-29 11:38:24.946914
11579	4750	1	16	16_RoboDrill4_202501.CSV	2025-04-29 11:38:24.946914
11580	4751	1	16	16_RoboDrill4_202501.CSV	2025-04-29 11:38:24.946914
11581	4752	1	16	16_RoboDrill4_202501.CSV	2025-04-29 11:38:24.946914
11582	4753	1	16	16_RoboDrill4_202501.CSV	2025-04-29 11:38:24.946914
11583	4754	1	16	16_RoboDrill4_202501.CSV	2025-04-29 11:38:24.946914
11584	4755	1	16	16_RoboDrill4_202501.CSV	2025-04-29 11:38:24.946914
11585	4756	1	16	16_RoboDrill4_202501.CSV	2025-04-29 11:38:24.946914
11586	4757	1	16	16_RoboDrill4_202501.CSV	2025-04-29 11:38:24.946914
11587	4758	1	16	16_RoboDrill4_202501.CSV	2025-04-29 11:38:24.946914
11588	4759	1	16	16_RoboDrill4_202501.CSV	2025-04-29 11:38:24.946914
11589	4760	1	16	16_RoboDrill4_202501.CSV	2025-04-29 11:38:24.946914
11590	4761	1	16	16_RoboDrill4_202501.CSV	2025-04-29 11:38:24.946914
11591	4762	1	16	16_RoboDrill4_202501.CSV	2025-04-29 11:38:24.946914
11592	4763	1	16	16_RoboDrill4_202501.CSV	2025-04-29 11:38:24.946914
11593	4764	1	16	16_RoboDrill4_202501.CSV	2025-04-29 11:38:24.946914
11594	4765	1	16	16_RoboDrill4_202501.CSV	2025-04-29 11:38:24.946914
11595	4766	1	16	16_RoboDrill4_202501.CSV	2025-04-29 11:38:24.946914
11596	4767	1	16	16_RoboDrill4_202501.CSV	2025-04-29 11:38:24.946914
11597	4768	1	16	16_RoboDrill4_202501.CSV	2025-04-29 11:38:24.946914
11598	4769	1	16	16_RoboDrill4_202501.CSV	2025-04-29 11:38:24.946914
11599	4770	1	16	16_RoboDrill4_202501.CSV	2025-04-29 11:38:24.946914
11600	4771	1	17	17_RoboDrill5_202501.CSV	2025-04-29 11:38:34.792155
11601	4772	1	17	17_RoboDrill5_202501.CSV	2025-04-29 11:38:34.792155
11602	4773	1	17	17_RoboDrill5_202501.CSV	2025-04-29 11:38:34.792155
11603	4774	1	17	17_RoboDrill5_202501.CSV	2025-04-29 11:38:34.792155
11604	4775	1	17	17_RoboDrill5_202501.CSV	2025-04-29 11:38:34.792155
11605	4776	1	17	17_RoboDrill5_202501.CSV	2025-04-29 11:38:34.792155
11606	4777	1	17	17_RoboDrill5_202501.CSV	2025-04-29 11:38:34.792155
11607	4778	1	17	17_RoboDrill5_202501.CSV	2025-04-29 11:38:34.792155
11608	4779	1	17	17_RoboDrill5_202501.CSV	2025-04-29 11:38:34.792155
11609	4780	1	17	17_RoboDrill5_202501.CSV	2025-04-29 11:38:34.792155
11610	4781	1	17	17_RoboDrill5_202501.CSV	2025-04-29 11:38:34.792155
11611	4782	1	17	17_RoboDrill5_202501.CSV	2025-04-29 11:38:34.792155
11612	4783	1	17	17_RoboDrill5_202501.CSV	2025-04-29 11:38:34.792155
11613	4784	1	17	17_RoboDrill5_202501.CSV	2025-04-29 11:38:34.792155
11614	4785	1	17	17_RoboDrill5_202501.CSV	2025-04-29 11:38:34.792155
11615	4786	1	17	17_RoboDrill5_202501.CSV	2025-04-29 11:38:34.792155
11616	4787	1	17	17_RoboDrill5_202501.CSV	2025-04-29 11:38:34.792155
11617	4788	1	17	17_RoboDrill5_202501.CSV	2025-04-29 11:38:34.792155
11618	4789	1	17	17_RoboDrill5_202501.CSV	2025-04-29 11:38:34.792155
11619	4790	1	17	17_RoboDrill5_202501.CSV	2025-04-29 11:38:34.792155
11620	4791	1	17	17_RoboDrill5_202501.CSV	2025-04-29 11:38:34.792155
11621	4792	1	17	17_RoboDrill5_202501.CSV	2025-04-29 11:38:34.792155
11622	4793	1	17	17_RoboDrill5_202501.CSV	2025-04-29 11:38:34.792155
11623	4794	1	18	18_RoboDrill6_202501.CSV	2025-04-29 11:38:43.727986
11624	4795	1	18	18_RoboDrill6_202501.CSV	2025-04-29 11:38:43.727986
11625	4796	1	18	18_RoboDrill6_202501.CSV	2025-04-29 11:38:43.727986
11626	4797	1	18	18_RoboDrill6_202501.CSV	2025-04-29 11:38:43.727986
11627	4798	1	18	18_RoboDrill6_202501.CSV	2025-04-29 11:38:43.727986
11628	4799	1	18	18_RoboDrill6_202501.CSV	2025-04-29 11:38:43.727986
11629	4800	1	18	18_RoboDrill6_202501.CSV	2025-04-29 11:38:43.727986
11630	4801	1	18	18_RoboDrill6_202501.CSV	2025-04-29 11:38:43.727986
11631	4802	1	18	18_RoboDrill6_202501.CSV	2025-04-29 11:38:43.727986
11632	4803	1	18	18_RoboDrill6_202501.CSV	2025-04-29 11:38:43.727986
11633	4804	1	18	18_RoboDrill6_202501.CSV	2025-04-29 11:38:43.727986
11634	4805	1	18	18_RoboDrill6_202501.CSV	2025-04-29 11:38:43.727986
11635	4806	1	18	18_RoboDrill6_202501.CSV	2025-04-29 11:38:43.727986
11636	4807	1	18	18_RoboDrill6_202501.CSV	2025-04-29 11:38:43.727986
11637	4808	1	18	18_RoboDrill6_202501.CSV	2025-04-29 11:38:43.727986
11638	4809	1	18	18_RoboDrill6_202501.CSV	2025-04-29 11:38:43.727986
11639	4810	1	18	18_RoboDrill6_202501.CSV	2025-04-29 11:38:43.727986
11640	4811	1	18	18_RoboDrill6_202501.CSV	2025-04-29 11:38:43.727986
11641	4812	1	18	18_RoboDrill6_202501.CSV	2025-04-29 11:38:43.727986
11642	4813	1	18	18_RoboDrill6_202501.CSV	2025-04-29 11:38:43.727986
11643	4814	1	18	18_RoboDrill6_202501.CSV	2025-04-29 11:38:43.727986
11644	4815	1	18	18_RoboDrill6_202501.CSV	2025-04-29 11:38:43.727986
11645	4816	1	18	18_RoboDrill6_202501.CSV	2025-04-29 11:38:43.727986
11646	4817	2	19	19_NC-Rooter1_202501.CSV	2025-04-29 11:38:55.161537
11647	4818	2	19	19_NC-Rooter1_202501.CSV	2025-04-29 11:38:55.161537
11648	4819	2	19	19_NC-Rooter1_202501.CSV	2025-04-29 11:38:55.161537
11649	4820	2	19	19_NC-Rooter1_202501.CSV	2025-04-29 11:38:55.161537
11650	4821	2	19	19_NC-Rooter1_202501.CSV	2025-04-29 11:38:55.161537
11651	4822	2	19	19_NC-Rooter1_202501.CSV	2025-04-29 11:38:55.161537
11652	4823	2	19	19_NC-Rooter1_202501.CSV	2025-04-29 11:38:55.161537
11653	4824	2	19	19_NC-Rooter1_202501.CSV	2025-04-29 11:38:55.161537
11654	4825	2	19	19_NC-Rooter1_202501.CSV	2025-04-29 11:38:55.161537
11655	4826	2	19	19_NC-Rooter1_202501.CSV	2025-04-29 11:38:55.161537
11656	4827	2	19	19_NC-Rooter1_202501.CSV	2025-04-29 11:38:55.161537
11657	4828	2	19	19_NC-Rooter1_202501.CSV	2025-04-29 11:38:55.161537
11658	4829	2	19	19_NC-Rooter1_202501.CSV	2025-04-29 11:38:55.161537
11659	4830	2	19	19_NC-Rooter1_202501.CSV	2025-04-29 11:38:55.161537
11660	4831	2	19	19_NC-Rooter1_202501.CSV	2025-04-29 11:38:55.161537
11661	4832	2	19	19_NC-Rooter1_202501.CSV	2025-04-29 11:38:55.161537
11662	4833	2	19	19_NC-Rooter1_202501.CSV	2025-04-29 11:38:55.161537
11663	4834	2	19	19_NC-Rooter1_202501.CSV	2025-04-29 11:38:55.161537
11664	4835	2	19	19_NC-Rooter1_202501.CSV	2025-04-29 11:38:55.161537
11665	4836	2	19	19_NC-Rooter1_202501.CSV	2025-04-29 11:38:55.161537
11666	4837	2	19	19_NC-Rooter1_202501.CSV	2025-04-29 11:38:55.161537
11667	4838	2	19	19_NC-Rooter1_202501.CSV	2025-04-29 11:38:55.161537
11668	4839	2	19	19_NC-Rooter1_202501.CSV	2025-04-29 11:38:55.161537
11669	4840	2	20	20_NC-Rooter2_202501.CSV	2025-04-29 11:39:03.69299
11670	4841	2	20	20_NC-Rooter2_202501.CSV	2025-04-29 11:39:03.69299
11671	4842	2	20	20_NC-Rooter2_202501.CSV	2025-04-29 11:39:03.69299
11672	4843	2	20	20_NC-Rooter2_202501.CSV	2025-04-29 11:39:03.69299
11673	4844	2	20	20_NC-Rooter2_202501.CSV	2025-04-29 11:39:03.69299
11674	4845	2	20	20_NC-Rooter2_202501.CSV	2025-04-29 11:39:03.69299
11675	4846	2	20	20_NC-Rooter2_202501.CSV	2025-04-29 11:39:03.69299
11676	4847	2	20	20_NC-Rooter2_202501.CSV	2025-04-29 11:39:03.69299
11677	4848	2	20	20_NC-Rooter2_202501.CSV	2025-04-29 11:39:03.69299
11678	4849	2	20	20_NC-Rooter2_202501.CSV	2025-04-29 11:39:03.69299
11679	4850	2	20	20_NC-Rooter2_202501.CSV	2025-04-29 11:39:03.69299
11680	4851	2	20	20_NC-Rooter2_202501.CSV	2025-04-29 11:39:03.69299
11681	4852	2	20	20_NC-Rooter2_202501.CSV	2025-04-29 11:39:03.69299
11682	4853	2	20	20_NC-Rooter2_202501.CSV	2025-04-29 11:39:03.69299
11683	4854	2	20	20_NC-Rooter2_202501.CSV	2025-04-29 11:39:03.69299
11684	4855	2	20	20_NC-Rooter2_202501.CSV	2025-04-29 11:39:03.69299
11685	4856	2	20	20_NC-Rooter2_202501.CSV	2025-04-29 11:39:03.69299
11686	4857	2	20	20_NC-Rooter2_202501.CSV	2025-04-29 11:39:03.69299
11687	4858	2	20	20_NC-Rooter2_202501.CSV	2025-04-29 11:39:03.69299
11688	4859	2	20	20_NC-Rooter2_202501.CSV	2025-04-29 11:39:03.69299
11689	4860	2	20	20_NC-Rooter2_202501.CSV	2025-04-29 11:39:03.69299
11690	4861	2	20	20_NC-Rooter2_202501.CSV	2025-04-29 11:39:03.69299
11691	4862	2	20	20_NC-Rooter2_202501.CSV	2025-04-29 11:39:03.69299
11692	4863	2	21	21_NC-Rooter3_202501.CSV	2025-04-29 11:39:12.372955
11693	4864	2	21	21_NC-Rooter3_202501.CSV	2025-04-29 11:39:12.372955
11694	4865	2	21	21_NC-Rooter3_202501.CSV	2025-04-29 11:39:12.372955
11695	4866	2	21	21_NC-Rooter3_202501.CSV	2025-04-29 11:39:12.372955
11696	4867	2	21	21_NC-Rooter3_202501.CSV	2025-04-29 11:39:12.372955
11697	4868	2	21	21_NC-Rooter3_202501.CSV	2025-04-29 11:39:12.372955
11698	4869	2	21	21_NC-Rooter3_202501.CSV	2025-04-29 11:39:12.372955
11699	4870	2	21	21_NC-Rooter3_202501.CSV	2025-04-29 11:39:12.372955
11700	4871	2	21	21_NC-Rooter3_202501.CSV	2025-04-29 11:39:12.372955
11701	4872	2	21	21_NC-Rooter3_202501.CSV	2025-04-29 11:39:12.372955
11702	4873	2	21	21_NC-Rooter3_202501.CSV	2025-04-29 11:39:12.372955
11703	4874	2	21	21_NC-Rooter3_202501.CSV	2025-04-29 11:39:12.372955
11704	4875	2	21	21_NC-Rooter3_202501.CSV	2025-04-29 11:39:12.372955
11705	4876	2	21	21_NC-Rooter3_202501.CSV	2025-04-29 11:39:12.372955
11706	4877	2	21	21_NC-Rooter3_202501.CSV	2025-04-29 11:39:12.372955
11707	4878	2	21	21_NC-Rooter3_202501.CSV	2025-04-29 11:39:12.372955
11708	4879	2	21	21_NC-Rooter3_202501.CSV	2025-04-29 11:39:12.372955
11709	4880	2	21	21_NC-Rooter3_202501.CSV	2025-04-29 11:39:12.372955
11710	4881	2	21	21_NC-Rooter3_202501.CSV	2025-04-29 11:39:12.372955
11711	4882	2	21	21_NC-Rooter3_202501.CSV	2025-04-29 11:39:12.372955
11712	4883	2	21	21_NC-Rooter3_202501.CSV	2025-04-29 11:39:12.372955
11713	4884	2	21	21_NC-Rooter3_202501.CSV	2025-04-29 11:39:12.372955
11714	4885	2	21	21_NC-Rooter3_202501.CSV	2025-04-29 11:39:12.372955
11715	9327	2	21	DataLog_HMI21_20250401.csv	2025-04-29 14:52:23.224123
11716	9328	2	21	DataLog_HMI21_20250401.csv	2025-04-29 14:52:23.224123
11717	9329	2	21	DataLog_HMI21_20250401.csv	2025-04-29 14:52:23.224123
11718	9330	2	21	DataLog_HMI21_20250401.csv	2025-04-29 14:52:23.224123
11719	9331	2	21	DataLog_HMI21_20250401.csv	2025-04-29 14:52:23.224123
11720	9332	2	21	DataLog_HMI21_20250401.csv	2025-04-29 14:52:23.224123
11721	9333	2	21	DataLog_HMI21_20250401.csv	2025-04-29 14:52:23.224123
11722	9334	2	21	DataLog_HMI21_20250401.csv	2025-04-29 14:52:23.224123
11723	9335	2	21	DataLog_HMI21_20250401.csv	2025-04-29 14:52:23.224123
11724	9336	2	21	DataLog_HMI21_20250401.csv	2025-04-29 14:52:23.224123
11725	9337	2	21	DataLog_HMI21_20250401.csv	2025-04-29 14:52:23.224123
11726	9338	2	21	DataLog_HMI21_20250401.csv	2025-04-29 14:52:23.224123
11727	9339	2	21	DataLog_HMI21_20250401.csv	2025-04-29 14:52:23.224123
11728	9340	2	21	DataLog_HMI21_20250401.csv	2025-04-29 14:52:23.224123
11729	9341	2	21	DataLog_HMI21_20250401.csv	2025-04-29 14:52:23.224123
11730	9342	1	1	DataLog_HMI1_20250401.csv	2025-04-29 16:45:11.643833
11731	9343	1	1	DataLog_HMI1_20250401.csv	2025-04-29 16:45:11.643833
11732	9344	1	1	DataLog_HMI1_20250401.csv	2025-04-29 16:45:11.643833
11733	9345	1	1	DataLog_HMI1_20250401.csv	2025-04-29 16:45:11.643833
11734	9346	1	1	DataLog_HMI1_20250401.csv	2025-04-29 16:45:11.643833
11735	9347	1	1	DataLog_HMI1_20250401.csv	2025-04-29 16:45:11.643833
11736	9348	1	1	DataLog_HMI1_20250401.csv	2025-04-29 16:45:11.643833
11737	9349	1	1	DataLog_HMI1_20250401.csv	2025-04-29 16:45:11.643833
11738	9350	1	1	DataLog_HMI1_20250401.csv	2025-04-29 16:45:11.643833
11739	9351	1	1	DataLog_HMI1_20250401.csv	2025-04-29 16:45:11.643833
11740	9352	1	1	DataLog_HMI1_20250401.csv	2025-04-29 16:45:11.643833
11741	9353	1	1	DataLog_HMI1_20250401.csv	2025-04-29 16:45:11.643833
11742	9354	1	1	DataLog_HMI1_20250401.csv	2025-04-29 16:45:11.643833
11743	9355	1	1	DataLog_HMI1_20250401.csv	2025-04-29 16:45:11.643833
11744	9356	1	1	DataLog_HMI1_20250401.csv	2025-04-29 16:45:11.643833
11745	4886	1	1	1_NV4000_202501.CSV	2025-04-29 16:45:37.818214
11746	4887	1	1	1_NV4000_202501.CSV	2025-04-29 16:45:37.818214
11747	4888	1	1	1_NV4000_202501.CSV	2025-04-29 16:45:37.818214
11748	4889	1	1	1_NV4000_202501.CSV	2025-04-29 16:45:37.818214
11749	4890	1	1	1_NV4000_202501.CSV	2025-04-29 16:45:37.818214
11750	4891	1	1	1_NV4000_202501.CSV	2025-04-29 16:45:37.818214
11751	4892	1	1	1_NV4000_202501.CSV	2025-04-29 16:45:37.818214
11752	4893	1	1	1_NV4000_202501.CSV	2025-04-29 16:45:37.818214
11753	4894	1	1	1_NV4000_202501.CSV	2025-04-29 16:45:37.818214
11754	4895	1	1	1_NV4000_202501.CSV	2025-04-29 16:45:37.818214
11755	4896	1	1	1_NV4000_202501.CSV	2025-04-29 16:45:37.818214
11756	4897	1	1	1_NV4000_202501.CSV	2025-04-29 16:45:37.818214
11757	4898	1	1	1_NV4000_202501.CSV	2025-04-29 16:45:37.818214
11758	4899	1	1	1_NV4000_202501.CSV	2025-04-29 16:45:37.818214
11759	4900	1	1	1_NV4000_202501.CSV	2025-04-29 16:45:37.818214
11760	4901	1	1	1_NV4000_202501.CSV	2025-04-29 16:45:37.818214
11761	4902	1	1	1_NV4000_202501.CSV	2025-04-29 16:45:37.818214
11762	4903	1	1	1_NV4000_202501.CSV	2025-04-29 16:45:37.818214
11763	4904	1	1	1_NV4000_202501.CSV	2025-04-29 16:45:37.818214
11764	4905	1	1	1_NV4000_202501.CSV	2025-04-29 16:45:37.818214
11765	4906	1	1	1_NV4000_202501.CSV	2025-04-29 16:45:37.818214
11766	4907	1	1	1_NV4000_202501.CSV	2025-04-29 16:45:37.818214
11767	4908	1	1	1_NV4000_202501.CSV	2025-04-29 16:45:37.818214
11768	4909	2	21	21_NC-Rooter3_202501.CSV	2025-04-29 16:46:57.458144
11769	4910	2	21	21_NC-Rooter3_202501.CSV	2025-04-29 16:46:57.458144
11770	4911	2	21	21_NC-Rooter3_202501.CSV	2025-04-29 16:46:57.458144
11771	4912	2	21	21_NC-Rooter3_202501.CSV	2025-04-29 16:46:57.458144
11772	4913	2	21	21_NC-Rooter3_202501.CSV	2025-04-29 16:46:57.458144
11773	4914	2	21	21_NC-Rooter3_202501.CSV	2025-04-29 16:46:57.458144
11774	4915	2	21	21_NC-Rooter3_202501.CSV	2025-04-29 16:46:57.458144
11775	4916	2	21	21_NC-Rooter3_202501.CSV	2025-04-29 16:46:57.458144
11776	4917	2	21	21_NC-Rooter3_202501.CSV	2025-04-29 16:46:57.458144
11777	4918	2	21	21_NC-Rooter3_202501.CSV	2025-04-29 16:46:57.458144
11778	4919	2	21	21_NC-Rooter3_202501.CSV	2025-04-29 16:46:57.458144
11779	4920	2	21	21_NC-Rooter3_202501.CSV	2025-04-29 16:46:57.458144
11780	4921	2	21	21_NC-Rooter3_202501.CSV	2025-04-29 16:46:57.458144
11781	4922	2	21	21_NC-Rooter3_202501.CSV	2025-04-29 16:46:57.458144
11782	4923	2	21	21_NC-Rooter3_202501.CSV	2025-04-29 16:46:57.458144
11783	4924	2	21	21_NC-Rooter3_202501.CSV	2025-04-29 16:46:57.458144
11784	4925	2	21	21_NC-Rooter3_202501.CSV	2025-04-29 16:46:57.458144
11785	4926	2	21	21_NC-Rooter3_202501.CSV	2025-04-29 16:46:57.458144
11786	4927	2	21	21_NC-Rooter3_202501.CSV	2025-04-29 16:46:57.458144
11787	4928	2	21	21_NC-Rooter3_202501.CSV	2025-04-29 16:46:57.458144
11788	4929	2	21	21_NC-Rooter3_202501.CSV	2025-04-29 16:46:57.458144
11789	4930	2	21	21_NC-Rooter3_202501.CSV	2025-04-29 16:46:57.458144
11790	4931	2	21	21_NC-Rooter3_202501.CSV	2025-04-29 16:46:57.458144
10623	3389	1	1	1_NV4000_202504.CSV	2025-04-23 08:41:30.64173
10625	3391	1	1	1_NV4000_202504.CSV	2025-04-23 08:41:30.64173
10627	3393	1	1	1_NV4000_202504.CSV	2025-04-23 08:41:30.64173
10629	3395	1	1	1_NV4000_202504.CSV	2025-04-23 08:41:30.64173
10631	3397	1	1	1_NV4000_202504.CSV	2025-04-23 08:41:30.64173
10633	3399	1	1	1_NV4000_202504.CSV	2025-04-23 08:41:30.64173
10635	3400	1	1	1_NV4000_202504.CSV	2025-04-23 08:41:30.64173
10637	3402	1	1	1_NV4000_202504.CSV	2025-04-23 08:41:30.64173
10639	3405	1	1	1_NV4000_202504.CSV	2025-04-23 08:41:30.64173
10641	3407	1	1	1_NV4000_202504.CSV	2025-04-23 08:41:30.64173
10643	3408	1	1	1_NV4000_202504.CSV	2025-04-23 08:41:30.64173
10645	3410	1	1	1_NV4000_202504.CSV	2025-04-23 08:41:30.64173
10647	3412	1	1	1_NV4000_202504.CSV	2025-04-23 08:41:30.64173
10649	3415	1	1	1_NV4000_202504.CSV	2025-04-23 08:41:30.64173
10624	3390	1	1	1_NV4000_202503.CSV	2025-04-23 08:41:30.641725
10626	3392	1	1	1_NV4000_202503.CSV	2025-04-23 08:41:30.641725
10628	3394	1	1	1_NV4000_202503.CSV	2025-04-23 08:41:30.641725
10630	3396	1	1	1_NV4000_202503.CSV	2025-04-23 08:41:30.641725
10632	3398	1	1	1_NV4000_202503.CSV	2025-04-23 08:41:30.641725
10634	3401	1	1	1_NV4000_202503.CSV	2025-04-23 08:41:30.641725
10636	3403	1	1	1_NV4000_202503.CSV	2025-04-23 08:41:30.641725
10638	3404	1	1	1_NV4000_202503.CSV	2025-04-23 08:41:30.641725
10640	3406	1	1	1_NV4000_202503.CSV	2025-04-23 08:41:30.641725
10642	3409	1	1	1_NV4000_202503.CSV	2025-04-23 08:41:30.641725
10644	3411	1	1	1_NV4000_202503.CSV	2025-04-23 08:41:30.641725
10646	3413	1	1	1_NV4000_202503.CSV	2025-04-23 08:41:30.641725
10648	3414	1	1	1_NV4000_202503.CSV	2025-04-23 08:41:30.641725
10650	3416	1	1	1_NV4000_202503.CSV	2025-04-23 08:41:30.641725
10651	3417	1	1	1_NV4000_202503.CSV	2025-04-23 08:41:30.641725
10652	3418	1	1	1_NV4000_202503.CSV	2025-04-23 08:41:30.641725
10653	3419	1	1	1_NV4000_202503.CSV	2025-04-23 08:41:30.641725
10654	3420	1	1	1_NV4000_202503.CSV	2025-04-23 08:41:30.641725
10655	3421	1	1	1_NV4000_202503.CSV	2025-04-23 08:41:30.641725
10656	3422	1	1	1_NV4000_202503.CSV	2025-04-23 08:41:30.641725
10657	3423	1	1	1_NV4000_202503.CSV	2025-04-23 08:41:30.641725
10658	9048	1	1	DataLog_HMI1_20250301.csv	2025-04-23 08:45:25.039467
10659	9049	1	1	DataLog_HMI1_20250301.csv	2025-04-23 08:45:25.039467
10660	9050	1	1	DataLog_HMI1_20250301.csv	2025-04-23 08:45:25.039467
10661	9051	1	1	DataLog_HMI1_20250301.csv	2025-04-23 08:45:25.039467
10662	9052	1	1	DataLog_HMI1_20250301.csv	2025-04-23 08:45:25.039467
10663	9053	1	1	DataLog_HMI1_20250301.csv	2025-04-23 08:45:25.039467
10664	9054	1	1	DataLog_HMI1_20250301.csv	2025-04-23 08:45:25.039467
10665	9055	1	1	DataLog_HMI1_20250301.csv	2025-04-23 08:45:25.039467
10666	9056	1	1	DataLog_HMI1_20250301.csv	2025-04-23 08:45:25.039467
10667	9057	1	1	DataLog_HMI1_20250301.csv	2025-04-23 08:45:25.039467
10668	9058	1	1	DataLog_HMI1_20250301.csv	2025-04-23 08:45:25.039467
10669	9059	1	1	DataLog_HMI1_20250301.csv	2025-04-23 08:45:25.039467
10670	9060	1	1	DataLog_HMI1_20250301.csv	2025-04-23 08:45:25.039467
10671	9061	1	1	DataLog_HMI1_20250301.csv	2025-04-23 08:45:25.039467
10672	9062	1	1	DataLog_HMI1_20250301.csv	2025-04-23 08:45:25.039467
10673	9063	1	1	DataLog_HMI1_20250301.csv	2025-04-23 08:50:44.709752
10674	9064	1	1	DataLog_HMI1_20250301.csv	2025-04-23 08:50:44.709752
10675	9065	1	1	DataLog_HMI1_20250301.csv	2025-04-23 08:50:44.709752
10676	9066	1	1	DataLog_HMI1_20250301.csv	2025-04-23 08:50:44.709752
10677	9067	1	1	DataLog_HMI1_20250301.csv	2025-04-23 08:50:44.709752
10678	9068	1	1	DataLog_HMI1_20250301.csv	2025-04-23 08:50:44.709752
10679	9069	1	1	DataLog_HMI1_20250301.csv	2025-04-23 08:50:44.709752
10680	9070	1	1	DataLog_HMI1_20250301.csv	2025-04-23 08:50:44.709752
10681	9071	1	1	DataLog_HMI1_20250301.csv	2025-04-23 08:50:44.709752
10682	9072	1	1	DataLog_HMI1_20250301.csv	2025-04-23 08:50:44.709752
10683	9073	1	1	DataLog_HMI1_20250301.csv	2025-04-23 08:50:44.709752
10684	9074	1	1	DataLog_HMI1_20250301.csv	2025-04-23 08:50:44.709752
10685	9075	1	1	DataLog_HMI1_20250301.csv	2025-04-23 08:50:44.709752
10686	9076	1	1	DataLog_HMI1_20250301.csv	2025-04-23 08:50:44.709752
10687	9077	1	1	DataLog_HMI1_20250301.csv	2025-04-23 08:50:44.709752
10688	9078	1	1	DataLog_HMI1_20250301.csv	2025-04-23 08:50:59.33533
10689	9079	1	1	DataLog_HMI1_20250301.csv	2025-04-23 08:50:59.33533
10690	9080	1	1	DataLog_HMI1_20250301.csv	2025-04-23 08:50:59.33533
10691	9081	1	1	DataLog_HMI1_20250301.csv	2025-04-23 08:50:59.33533
10692	9082	1	1	DataLog_HMI1_20250301.csv	2025-04-23 08:50:59.33533
10693	9083	1	1	DataLog_HMI1_20250301.csv	2025-04-23 08:50:59.33533
10694	9084	1	1	DataLog_HMI1_20250301.csv	2025-04-23 08:50:59.33533
10695	9085	1	1	DataLog_HMI1_20250301.csv	2025-04-23 08:50:59.33533
10696	9086	1	1	DataLog_HMI1_20250301.csv	2025-04-23 08:50:59.33533
10697	9087	1	1	DataLog_HMI1_20250301.csv	2025-04-23 08:50:59.33533
10698	9088	1	1	DataLog_HMI1_20250301.csv	2025-04-23 08:50:59.33533
10699	9089	1	1	DataLog_HMI1_20250301.csv	2025-04-23 08:50:59.33533
10700	9090	1	1	DataLog_HMI1_20250301.csv	2025-04-23 08:50:59.33533
10701	9091	1	1	DataLog_HMI1_20250301.csv	2025-04-23 08:50:59.33533
10702	9092	1	1	DataLog_HMI1_20250301.csv	2025-04-23 08:50:59.33533
10703	3424	1	1	1_NV4000_202504.CSV	2025-04-23 08:55:42.358459
10704	3425	1	1	1_NV4000_202504.CSV	2025-04-23 08:55:42.358459
10705	3426	1	1	1_NV4000_202504.CSV	2025-04-23 08:55:42.358459
10706	3427	1	1	1_NV4000_202504.CSV	2025-04-23 08:55:42.358459
10707	3428	1	1	1_NV4000_202504.CSV	2025-04-23 08:55:42.358459
10708	3429	1	1	1_NV4000_202504.CSV	2025-04-23 08:55:42.358459
10709	3430	1	1	1_NV4000_202504.CSV	2025-04-23 08:55:42.358459
10710	3431	1	1	1_NV4000_202504.CSV	2025-04-23 08:55:42.358459
10711	3432	1	1	1_NV4000_202504.CSV	2025-04-23 08:55:42.358459
10712	3433	1	1	1_NV4000_202504.CSV	2025-04-23 08:55:42.358459
10713	3434	1	1	1_NV4000_202504.CSV	2025-04-23 08:55:42.358459
10714	3435	1	1	1_NV4000_202504.CSV	2025-04-23 08:55:42.358459
10715	3436	1	1	1_NV4000_202504.CSV	2025-04-23 08:55:42.358459
10716	3437	1	1	1_NV4000_202504.CSV	2025-04-23 08:55:42.358459
10717	9093	1	1	DataLog_HMI1_20250409.csv	2025-04-23 08:55:55.304064
10718	9094	1	1	DataLog_HMI1_20250409.csv	2025-04-23 08:55:55.304064
10719	9095	1	1	DataLog_HMI1_20250409.csv	2025-04-23 08:55:55.304064
10720	9096	1	1	DataLog_HMI1_20250409.csv	2025-04-23 08:55:55.304064
10721	9097	1	1	DataLog_HMI1_20250409.csv	2025-04-23 08:55:55.304064
10722	9098	1	1	DataLog_HMI1_20250409.csv	2025-04-23 08:55:55.304064
10723	9099	1	1	DataLog_HMI1_20250409.csv	2025-04-23 08:55:55.304064
10724	9100	1	1	DataLog_HMI1_20250409.csv	2025-04-23 08:55:55.304064
10725	9101	1	1	DataLog_HMI1_20250409.csv	2025-04-23 08:55:55.304064
10726	9102	1	1	DataLog_HMI1_20250409.csv	2025-04-23 08:55:55.304064
10727	9103	1	1	DataLog_HMI1_20250409.csv	2025-04-23 08:55:55.304064
10728	9104	1	1	DataLog_HMI1_20250409.csv	2025-04-23 08:55:55.304064
10729	9105	1	1	DataLog_HMI1_20250409.csv	2025-04-23 08:55:55.304064
10730	9106	1	1	DataLog_HMI1_20250409.csv	2025-04-23 08:55:55.304064
10731	9107	1	1	DataLog_HMI1_20250409.csv	2025-04-23 08:55:55.304064
10732	9108	1	1	DataLog_HMI1_20250409.csv	2025-04-23 08:55:55.304064
10734	9109	1	1	DataLog_HMI1_20250401.csv	2025-04-23 08:55:55.321949
10735	9110	1	1	DataLog_HMI1_20250401.csv	2025-04-23 08:55:55.321949
10737	9111	1	1	DataLog_HMI1_20250401.csv	2025-04-23 08:55:55.321949
10740	9112	1	1	DataLog_HMI1_20250401.csv	2025-04-23 08:55:55.321949
10742	9113	1	1	DataLog_HMI1_20250401.csv	2025-04-23 08:55:55.321949
10743	9114	1	1	DataLog_HMI1_20250401.csv	2025-04-23 08:55:55.321949
10746	9115	1	1	DataLog_HMI1_20250401.csv	2025-04-23 08:55:55.321949
10748	9116	1	1	DataLog_HMI1_20250401.csv	2025-04-23 08:55:55.321949
10750	9117	1	1	DataLog_HMI1_20250401.csv	2025-04-23 08:55:55.321949
10752	9118	1	1	DataLog_HMI1_20250401.csv	2025-04-23 08:55:55.321949
10754	9119	1	1	DataLog_HMI1_20250401.csv	2025-04-23 08:55:55.321949
10756	9120	1	1	DataLog_HMI1_20250401.csv	2025-04-23 08:55:55.321949
10758	9121	1	1	DataLog_HMI1_20250401.csv	2025-04-23 08:55:55.321949
10760	9122	1	1	DataLog_HMI1_20250401.csv	2025-04-23 08:55:55.321949
10762	9123	1	1	DataLog_HMI1_20250401.csv	2025-04-23 08:55:55.321949
10733	9124	1	1	DataLog_HMI1_20250408.csv	2025-04-23 08:55:55.322399
10736	9125	1	1	DataLog_HMI1_20250408.csv	2025-04-23 08:55:55.322399
10738	9126	1	1	DataLog_HMI1_20250408.csv	2025-04-23 08:55:55.322399
10739	9127	1	1	DataLog_HMI1_20250408.csv	2025-04-23 08:55:55.322399
10741	9128	1	1	DataLog_HMI1_20250408.csv	2025-04-23 08:55:55.322399
10744	9129	1	1	DataLog_HMI1_20250408.csv	2025-04-23 08:55:55.322399
10745	9130	1	1	DataLog_HMI1_20250408.csv	2025-04-23 08:55:55.322399
10747	9131	1	1	DataLog_HMI1_20250408.csv	2025-04-23 08:55:55.322399
10749	9132	1	1	DataLog_HMI1_20250408.csv	2025-04-23 08:55:55.322399
10751	9133	1	1	DataLog_HMI1_20250408.csv	2025-04-23 08:55:55.322399
10753	9134	1	1	DataLog_HMI1_20250408.csv	2025-04-23 08:55:55.322399
10755	9135	1	1	DataLog_HMI1_20250408.csv	2025-04-23 08:55:55.322399
10757	9136	1	1	DataLog_HMI1_20250408.csv	2025-04-23 08:55:55.322399
10759	9137	1	1	DataLog_HMI1_20250408.csv	2025-04-23 08:55:55.322399
10761	9138	1	1	DataLog_HMI1_20250408.csv	2025-04-23 08:55:55.322399
10763	9139	1	1	DataLog_HMI1_20250408.csv	2025-04-23 08:55:55.322399
10764	9140	1	1	DataLog_HMI1_20250408.csv	2025-04-23 08:55:55.322399
10765	9141	1	1	DataLog_HMI1_20250408.csv	2025-04-23 08:55:55.322399
10766	3438	2	2	2_NV5000_202504.CSV	2025-04-23 09:15:48.886001
10767	3439	2	2	2_NV5000_202504.CSV	2025-04-23 09:15:48.886001
10768	3440	2	2	2_NV5000_202504.CSV	2025-04-23 09:15:48.886001
10769	3441	2	2	2_NV5000_202504.CSV	2025-04-23 09:15:48.886001
10770	3442	2	2	2_NV5000_202504.CSV	2025-04-23 09:15:48.886001
10771	3443	2	2	2_NV5000_202504.CSV	2025-04-23 09:15:48.886001
10772	3444	2	2	2_NV5000_202504.CSV	2025-04-23 09:15:48.886001
10773	3445	2	2	2_NV5000_202504.CSV	2025-04-23 09:15:48.886001
10774	3446	2	2	2_NV5000_202504.CSV	2025-04-23 09:15:48.886001
10775	3447	2	2	2_NV5000_202504.CSV	2025-04-23 09:15:48.886001
10776	3448	2	2	2_NV5000_202504.CSV	2025-04-23 09:15:48.886001
10777	3449	2	2	2_NV5000_202504.CSV	2025-04-23 09:15:48.886001
10778	3450	2	2	2_NV5000_202504.CSV	2025-04-23 09:15:48.886001
10779	3451	2	2	2_NV5000_202504.CSV	2025-04-23 09:15:48.886001
10780	3452	2	2	2_NV5000_202504.CSV	2025-04-23 09:15:48.886001
\.


--
-- TOC entry 3380 (class 0 OID 12231661)
-- Dependencies: 233
-- Data for Name: TRN_OPERATION_OEE; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."TRN_OPERATION_OEE" ("ID", "FACTORY_CD", "SHIFT_ID", "LINE_ID", "PROCESS_ID", "MACHINE_NO", "ACHIEVEMENT_REGISTRATION_DATE", "ACHIEVEMENT_REGISTRATION_TIME", "PROCESSING_TIME", "PROCESSING_STOP_TIME", "LOSS_STOP_TIME", "PRODUCTION_COUNT", "OPERATION_RATE", "EQUIPMENT_OPERATION_HOURS", "LOAD_TIME", "TIME_OPERATING_RATE") FROM stdin;
3588	1	\N	\N	\N	03	2025-03-01	2025-03-01 20:00:18	7940	0	26036	8	0.00	9965	38842	25.70
3589	1	\N	\N	\N	03	2025-03-02	2025-03-02 20:00:17	11933	0	25358	13	0.00	11938	39520	30.20
3590	1	\N	\N	\N	03	2025-03-03	2025-03-03 20:00:18	8984	29	24853	4	0.00	8996	38589	23.30
3591	1	\N	\N	\N	03	2025-03-04	2025-03-04 20:00:18	12736	0	25101	6	0.00	12735	39142	32.50
3592	1	\N	\N	\N	03	2025-03-07	2025-03-07 20:00:18	14988	0	21191	9	0.00	15005	38122	39.40
3593	1	\N	\N	\N	03	2025-03-08	2025-03-08 20:00:18	19114	44	15770	7	0.00	19317	37667	51.30
3594	1	\N	\N	\N	03	2025-03-09	2025-03-09 20:00:18	11184	0	19424	18	0.00	11195	36156	31.00
3595	1	\N	\N	\N	03	2025-03-10	2025-03-10 20:00:19	9376	0	26194	4	0.00	9388	38366	24.50
3596	1	\N	\N	\N	03	2025-03-11	2025-03-11 20:00:17	10916	0	25842	13	0.00	10913	39916	27.30
3597	1	\N	\N	\N	03	2025-03-16	2025-03-16 20:00:18	15136	0	23881	4	0.00	15545	43288	35.90
3598	1	\N	\N	\N	03	2025-03-17	2025-03-17 20:00:18	11976	0	18697	5	0.00	11987	39281	30.50
3599	1	\N	\N	\N	03	2025-03-18	2025-03-18 20:00:18	10316	0	22946	8	0.00	10331	38873	26.60
3600	1	\N	\N	\N	03	2025-03-21	2025-03-21 20:00:18	10150	0	25832	8	0.00	10147	37188	27.30
3601	1	\N	\N	\N	03	2025-03-22	2025-03-22 20:00:17	15872	0	21013	10	0.00	15873	39325	40.40
3602	1	\N	\N	\N	03	2025-03-23	2025-03-23 20:00:18	20896	0	14438	9	0.00	20900	40854	51.20
3603	1	\N	\N	\N	03	2025-03-24	2025-03-24 20:00:19	18838	0	17957	9	0.00	19359	38469	50.30
3604	1	\N	\N	\N	03	2025-03-25	2025-03-25 20:00:19	14691	0	22806	5	0.00	14690	39448	37.20
3605	1	\N	\N	\N	03	2025-03-28	2025-03-28 20:00:18	7340	0	26910	9	0.00	7353	39491	18.60
3606	1	\N	\N	\N	03	2025-03-29	2025-03-29 20:00:17	9279	0	25081	10	0.00	9554	38878	24.60
3607	1	\N	\N	\N	03	2025-03-30	2025-03-30 20:00:17	12695	0	24367	6	0.00	12689	37131	34.20
3608	1	\N	\N	\N	03	2025-03-31	2025-03-31 20:00:18	12945	0	17362	11	0.00	13742	37696	36.50
3609	1	\N	\N	\N	03	2025-04-24	2025-04-24 20:00:18	12945	0	17362	11	0.00	13742	37696	36.50
3610	1	\N	\N	\N	03	2025-04-01	2025-04-01 20:00:18	7940	0	26036	28	1.00	9965	38842	33.70
3611	1	\N	\N	\N	03	2025-04-02	2025-04-02 20:00:17	11933	0	25358	2	2.00	11938	39520	30.20
3612	1	\N	\N	\N	03	2025-04-03	2025-04-03 20:00:18	8984	29	24853	3	3.00	8996	38589	27.60
3613	1	\N	\N	\N	03	2025-04-04	2025-04-04 20:00:18	12736	0	25101	4	4.00	12735	39142	22.50
3614	1	\N	\N	\N	03	2025-04-07	2025-04-07 20:00:18	14988	0	21191	5	7.00	15005	38122	39.40
3615	1	\N	\N	\N	03	2025-04-08	2025-04-08 20:00:18	19114	44	15770	6	8.00	19317	37667	51.30
3616	1	\N	\N	\N	03	2025-04-09	2025-04-09 20:00:18	11184	0	19424	7	9.00	11195	36156	31.00
3617	1	\N	\N	\N	03	2025-04-10	2025-04-10 20:00:19	9376	0	26194	8	10.00	9388	38366	24.50
3618	1	\N	\N	\N	03	2025-04-11	2025-04-11 20:00:17	10916	0	25842	9	11.00	10913	39916	27.30
3619	1	\N	\N	\N	03	2025-04-16	2025-04-16 20:00:18	15136	0	23881	10	16.00	15545	43288	35.90
3620	1	\N	\N	\N	03	2025-04-17	2025-04-17 20:00:18	11976	0	18697	11	17.00	11987	39281	30.50
3621	1	\N	\N	\N	03	2025-04-18	2025-04-18 20:00:18	10316	0	22946	12	18.00	10331	38873	26.60
3622	1	\N	\N	\N	03	2025-04-21	2025-04-21 20:00:18	10150	0	25832	13	21.00	10147	37188	27.30
3623	1	\N	\N	\N	03	2025-04-22	2025-04-22 20:00:17	15872	0	21013	14	22.00	15873	39325	40.40
5181	2	\N	\N	\N	20	2025-01-01	2025-01-01 20:00:18	7940	0	26036	2	0.00	9965	38842	20.40
3659	1	\N	\N	\N	04	2025-04-22	2025-04-22 20:00:17	15872	0	21013	14	22.00	15873	39325	40.40
3732	1	\N	\N	\N	07	2025-03-01	2025-03-01 20:00:18	7940	0	26036	8	0.00	9965	38842	25.70
3733	1	\N	\N	\N	07	2025-03-02	2025-03-02 20:00:17	11933	0	25358	13	0.00	11938	39520	30.20
3734	1	\N	\N	\N	07	2025-03-03	2025-03-03 20:00:18	8984	29	24853	4	0.00	8996	38589	23.30
3735	1	\N	\N	\N	07	2025-03-04	2025-03-04 20:00:18	12736	0	25101	6	0.00	12735	39142	32.50
3736	1	\N	\N	\N	07	2025-03-07	2025-03-07 20:00:18	14988	0	21191	9	0.00	15005	38122	39.40
3737	1	\N	\N	\N	07	2025-03-08	2025-03-08 20:00:18	19114	44	15770	7	0.00	19317	37667	51.30
3738	1	\N	\N	\N	07	2025-03-09	2025-03-09 20:00:18	11184	0	19424	18	0.00	11195	36156	31.00
3739	1	\N	\N	\N	07	2025-03-10	2025-03-10 20:00:19	9376	0	26194	4	0.00	9388	38366	24.50
3740	1	\N	\N	\N	07	2025-03-11	2025-03-11 20:00:17	10916	0	25842	13	0.00	10913	39916	27.30
3741	1	\N	\N	\N	07	2025-03-16	2025-03-16 20:00:18	15136	0	23881	4	0.00	15545	43288	35.90
3742	1	\N	\N	\N	07	2025-03-17	2025-03-17 20:00:18	11976	0	18697	5	0.00	11987	39281	30.50
3743	1	\N	\N	\N	07	2025-03-18	2025-03-18 20:00:18	10316	0	22946	8	0.00	10331	38873	26.60
3744	1	\N	\N	\N	07	2025-03-21	2025-03-21 20:00:18	10150	0	25832	8	0.00	10147	37188	27.30
3745	1	\N	\N	\N	07	2025-03-22	2025-03-22 20:00:17	15872	0	21013	10	0.00	15873	39325	40.40
3746	1	\N	\N	\N	07	2025-03-23	2025-03-23 20:00:18	20896	0	14438	9	0.00	20900	40854	51.20
5182	2	\N	\N	\N	20	2025-01-02	2025-01-02 20:00:17	4733	0	3758	2	0.00	11938	39520	11.90
5183	2	\N	\N	\N	20	2025-01-03	2025-01-03 20:00:18	8984	29	24853	5	0.00	8996	38589	23.20
5184	2	\N	\N	\N	20	2025-01-06	2025-01-06 20:00:18	4714	44	15770	2	0.00	19317	37667	12.50
5185	2	\N	\N	\N	20	2025-01-07	2025-01-07 20:00:18	11184	0	19424	3	0.00	11195	36156	30.90
5186	2	\N	\N	\N	20	2025-01-08	2025-01-08 20:00:19	9376	0	8194	1	0.00	9388	38366	24.40
5187	2	\N	\N	\N	20	2025-01-09	2025-01-09 20:00:17	10916	0	25842	0	0.00	10913	39916	27.30
5188	2	\N	\N	\N	20	2025-01-10	2025-01-10 20:00:18	15136	0	23881	3	0.00	15545	43288	34.90
5189	2	\N	\N	\N	20	2025-01-13	2025-01-13 20:00:18	10150	0	15032	15	0.00	10147	37188	27.20
5190	2	\N	\N	\N	20	2025-01-14	2025-01-14 20:00:17	15872	0	21013	10	0.00	15873	39325	40.30
5191	2	\N	\N	\N	20	2025-01-15	2025-01-15 20:00:18	20896	0	14438	33	0.00	20900	40854	51.10
5192	2	\N	\N	\N	20	2025-01-16	2025-01-16 20:00:19	17296	0	17957	12	0.00	19359	38469	4.90
5193	2	\N	\N	\N	20	2025-01-17	2025-01-17 20:00:19	14691	0	22806	1	0.00	14690	39448	37.20
5194	2	\N	\N	\N	20	2025-01-20	2025-01-20 20:00:17	12695	0	24367	0	0.00	12689	37131	34.10
5195	2	\N	\N	\N	20	2025-01-21	2025-01-21 20:00:18	12945	0	17362	2	0.00	13742	37696	34.30
5196	2	\N	\N	\N	20	2025-01-22	2025-01-22 20:00:18	5536	0	28701	33	0.00	12735	39142	14.10
5197	2	\N	\N	\N	20	2025-01-23	2025-01-23 20:00:18	14988	0	21191	1	0.00	15005	38122	39.30
5198	2	\N	\N	\N	20	2025-01-24	2025-01-24 20:00:18	11976	0	18697	1	0.00	11987	39281	30.40
5199	2	\N	\N	\N	20	2025-01-27	2025-01-27 20:00:17	9279	0	25081	2	0.00	9554	38878	23.80
5200	2	\N	\N	\N	20	2025-01-28	2025-01-28 20:00:18	10316	0	22946	8	0.00	10331	38873	26.50
5201	2	\N	\N	\N	20	2025-01-29	2025-01-29 20:00:18	7340	0	26910	5	0.00	7353	39491	18.50
5202	2	\N	\N	\N	20	2025-01-30	2025-01-30 20:00:19	18291	0	12006	5	0.00	14690	39448	46.30
5203	2	\N	\N	\N	20	2025-01-31	2025-01-31 20:00:17	10916	0	25842	9	0.00	10913	39916	27.30
3481	1	\N	\N	\N	02	2025-04-10	2025-04-10 20:00:19	9376	0	26194	8	10.00	9388	38366	24.50
5204	1	\N	\N	\N	01	2025-01-01	2025-01-01 20:00:18	7940	0	26036	8	0.00	9965	38842	20.40
5205	1	\N	\N	\N	01	2025-01-02	2025-01-02 20:00:17	11933	0	25358	13	0.00	11938	39520	30.10
5206	1	\N	\N	\N	01	2025-01-03	2025-01-03 20:00:18	8984	29	24853	4	0.00	8996	38589	23.20
5207	1	\N	\N	\N	01	2025-01-06	2025-01-06 20:00:18	19114	44	15770	7	0.00	19317	37667	50.70
5208	1	\N	\N	\N	01	2025-01-07	2025-01-07 20:00:18	11184	0	19424	18	0.00	11195	36156	30.90
5209	1	\N	\N	\N	01	2025-01-08	2025-01-08 20:00:19	9376	0	26194	4	0.00	9388	38366	24.40
5210	1	\N	\N	\N	01	2025-01-09	2025-01-09 20:00:17	10916	0	25842	13	0.00	10913	39916	27.30
5211	1	\N	\N	\N	01	2025-01-10	2025-01-10 20:00:18	15136	0	23881	4	0.00	15545	43288	34.90
5212	1	\N	\N	\N	01	2025-01-13	2025-01-13 20:00:18	10150	0	25832	8	0.00	10147	37188	27.20
5213	1	\N	\N	\N	01	2025-01-14	2025-01-14 20:00:17	15872	0	21013	10	0.00	15873	39325	40.30
5214	1	\N	\N	\N	01	2025-01-15	2025-01-15 20:00:18	20896	0	14438	9	0.00	20900	40854	51.10
5215	1	\N	\N	\N	01	2025-01-16	2025-01-16 20:00:19	18838	0	17957	9	0.00	19359	38469	48.90
5216	1	\N	\N	\N	01	2025-01-17	2025-01-17 20:00:19	14691	0	22806	5	0.00	14690	39448	37.20
3471	1	\N	\N	\N	01	2025-04-01	2025-04-01 20:00:18	7940	0	26036	28	1.00	9965	38842	33.70
3475	1	\N	\N	\N	01	2025-04-02	2025-04-02 20:00:17	11933	0	25358	2	2.00	11938	39520	30.20
3476	1	\N	\N	\N	01	2025-04-03	2025-04-03 20:00:18	8984	29	24853	3	3.00	8996	38589	27.60
3477	1	\N	\N	\N	01	2025-04-04	2025-04-04 20:00:18	12736	0	25101	4	4.00	12735	39142	22.50
3478	1	\N	\N	\N	01	2025-04-07	2025-04-07 20:00:18	14988	0	21191	5	7.00	15005	38122	39.40
3479	1	\N	\N	\N	01	2025-04-08	2025-04-08 20:00:18	19114	44	15770	6	8.00	19317	37667	51.30
3480	1	\N	\N	\N	01	2025-04-09	2025-04-09 20:00:18	11184	0	19424	7	9.00	11195	36156	31.00
3482	1	\N	\N	\N	01	2025-04-11	2025-04-11 20:00:17	10916	0	25842	9	11.00	10913	39916	27.30
3483	1	\N	\N	\N	01	2025-04-16	2025-04-16 20:00:18	15136	0	23881	10	16.00	15545	43288	35.90
3484	1	\N	\N	\N	01	2025-04-17	2025-04-17 20:00:18	11976	0	18697	11	17.00	11987	39281	30.50
3485	1	\N	\N	\N	01	2025-04-18	2025-04-18 20:00:18	10316	0	22946	12	18.00	10331	38873	26.60
3486	1	\N	\N	\N	01	2025-04-21	2025-04-21 20:00:18	10150	0	25832	13	21.00	10147	37188	27.30
3487	1	\N	\N	\N	01	2025-04-22	2025-04-22 20:00:17	15872	0	21013	14	22.00	15873	39325	40.40
3686	1	\N	\N	\N	05	2025-04-07	2025-04-07 20:00:18	14988	0	21191	5	7.00	15005	38122	39.40
3687	1	\N	\N	\N	05	2025-04-08	2025-04-08 20:00:18	19114	44	15770	6	8.00	19317	37667	51.30
3688	1	\N	\N	\N	05	2025-04-09	2025-04-09 20:00:18	11184	0	19424	7	9.00	11195	36156	31.00
3689	1	\N	\N	\N	05	2025-04-10	2025-04-10 20:00:19	9376	0	26194	8	10.00	9388	38366	24.50
3690	1	\N	\N	\N	05	2025-04-11	2025-04-11 20:00:17	10916	0	25842	9	11.00	10913	39916	27.30
3691	1	\N	\N	\N	05	2025-04-16	2025-04-16 20:00:18	15136	0	23881	10	16.00	15545	43288	35.90
3692	1	\N	\N	\N	05	2025-04-17	2025-04-17 20:00:18	11976	0	18697	11	17.00	11987	39281	30.50
5217	1	\N	\N	\N	01	2025-01-20	2025-01-20 20:00:17	12695	0	24367	6	0.00	12689	37131	34.10
5218	1	\N	\N	\N	01	2025-01-21	2025-01-21 20:00:18	12945	0	17362	11	0.00	13742	37696	34.40
5219	1	\N	\N	\N	01	2025-01-22	2025-01-22 20:00:18	12736	0	25101	6	0.00	12735	39142	32.50
5220	1	\N	\N	\N	01	2025-01-23	2025-01-23 20:00:18	14988	0	21191	9	0.00	15005	38122	39.30
5221	1	\N	\N	\N	01	2025-01-24	2025-01-24 20:00:18	11976	0	18697	5	0.00	11987	39281	30.40
5222	1	\N	\N	\N	01	2025-01-27	2025-01-27 20:00:17	9279	0	25081	10	0.00	9554	38878	23.80
5223	1	\N	\N	\N	01	2025-01-28	2025-01-28 20:00:18	10316	0	22946	8	0.00	10331	38873	26.50
5224	1	\N	\N	\N	01	2025-01-29	2025-01-29 20:00:18	7340	0	26910	9	0.00	7353	39491	18.50
5225	1	\N	\N	\N	01	2025-01-30	2025-01-30 20:00:19	14691	0	22806	5	0.00	14690	39448	37.20
5226	1	\N	\N	\N	01	2025-01-31	2025-01-31 20:00:17	10916	0	25842	13	0.00	10913	39916	27.30
5227	2	\N	\N	\N	02	2025-01-01	2025-01-01 20:00:18	7940	0	18836	1	0.00	9965	38842	11.10
5228	2	\N	\N	\N	02	2025-01-02	2025-01-02 20:00:17	11933	0	25358	1	0.00	11938	39520	30.10
3693	1	\N	\N	\N	05	2025-04-18	2025-04-18 20:00:18	10316	0	22946	12	18.00	10331	38873	26.60
3694	1	\N	\N	\N	05	2025-04-21	2025-04-21 20:00:18	10150	0	25832	13	21.00	10147	37188	27.30
3695	1	\N	\N	\N	05	2025-04-22	2025-04-22 20:00:17	15872	0	21013	14	22.00	15873	39325	40.40
4514	1	\N	\N	\N	05	2025-01-28	2025-01-28 20:00:18	10316	0	22946	12	0.00	10331	38873	26.50
4515	1	\N	\N	\N	05	2025-01-29	2025-01-29 20:00:18	3740	0	26910	12	0.00	7353	39491	9.40
4516	1	\N	\N	\N	05	2025-01-30	2025-01-30 20:00:19	14691	0	22806	15	0.00	14690	39448	37.20
4517	1	\N	\N	\N	05	2025-01-31	2025-01-31 20:00:17	3716	0	25842	13	0.00	10913	39916	9.30
3717	1	\N	\N	\N	06	2025-04-24	2025-04-24 20:00:18	12945	0	17362	11	0.00	13742	37696	36.50
3718	1	\N	\N	\N	06	2025-04-01	2025-04-01 20:00:18	7940	0	26036	28	1.00	9965	38842	33.70
3719	1	\N	\N	\N	06	2025-04-02	2025-04-02 20:00:17	11933	0	25358	2	2.00	11938	39520	30.20
3720	1	\N	\N	\N	06	2025-04-03	2025-04-03 20:00:18	8984	29	24853	3	3.00	8996	38589	27.60
3721	1	\N	\N	\N	06	2025-04-04	2025-04-04 20:00:18	12736	0	25101	4	4.00	12735	39142	22.50
3722	1	\N	\N	\N	06	2025-04-07	2025-04-07 20:00:18	14988	0	21191	5	7.00	15005	38122	39.40
3723	1	\N	\N	\N	06	2025-04-08	2025-04-08 20:00:18	19114	44	15770	6	8.00	19317	37667	51.30
3768	1	\N	\N	\N	08	2025-03-01	2025-03-01 20:00:18	7940	0	26036	8	0.00	9965	38842	25.70
3769	1	\N	\N	\N	08	2025-03-02	2025-03-02 20:00:17	11933	0	25358	13	0.00	11938	39520	30.20
3770	1	\N	\N	\N	08	2025-03-03	2025-03-03 20:00:18	8984	29	24853	4	0.00	8996	38589	23.30
3771	1	\N	\N	\N	08	2025-03-04	2025-03-04 20:00:18	12736	0	25101	6	0.00	12735	39142	32.50
3772	1	\N	\N	\N	08	2025-03-07	2025-03-07 20:00:18	14988	0	21191	9	0.00	15005	38122	39.40
3773	1	\N	\N	\N	08	2025-03-08	2025-03-08 20:00:18	19114	44	15770	7	0.00	19317	37667	51.30
3774	1	\N	\N	\N	08	2025-03-09	2025-03-09 20:00:18	11184	0	19424	18	0.00	11195	36156	31.00
3775	1	\N	\N	\N	08	2025-03-10	2025-03-10 20:00:19	9376	0	26194	4	0.00	9388	38366	24.50
3776	1	\N	\N	\N	08	2025-03-11	2025-03-11 20:00:17	10916	0	25842	13	0.00	10913	39916	27.30
3777	1	\N	\N	\N	08	2025-03-16	2025-03-16 20:00:18	15136	0	23881	4	0.00	15545	43288	35.90
3778	1	\N	\N	\N	08	2025-03-17	2025-03-17 20:00:18	11976	0	18697	5	0.00	11987	39281	30.50
3779	1	\N	\N	\N	08	2025-03-18	2025-03-18 20:00:18	10316	0	22946	8	0.00	10331	38873	26.60
3780	1	\N	\N	\N	08	2025-03-21	2025-03-21 20:00:18	10150	0	25832	8	0.00	10147	37188	27.30
5229	2	\N	\N	\N	02	2025-01-03	2025-01-03 20:00:18	8984	29	24853	1	0.00	8996	38589	23.20
5230	2	\N	\N	\N	02	2025-01-06	2025-01-06 20:00:18	4714	44	19370	2	0.00	19317	37667	12.50
5231	2	\N	\N	\N	02	2025-01-07	2025-01-07 20:00:18	3984	0	26624	30	0.00	11195	36156	11.00
5232	2	\N	\N	\N	02	2025-01-08	2025-01-08 20:00:19	9376	0	26194	4	0.00	9388	38366	24.40
5233	2	\N	\N	\N	02	2025-01-09	2025-01-09 20:00:17	10916	0	25842	2	0.00	10913	39916	27.30
5234	2	\N	\N	\N	02	2025-01-10	2025-01-10 20:00:18	15136	0	23881	3	0.00	15545	43288	34.90
5235	2	\N	\N	\N	02	2025-01-13	2025-01-13 20:00:18	6550	0	25832	4	0.00	10147	37188	17.60
5236	2	\N	\N	\N	02	2025-01-14	2025-01-14 20:00:17	5072	0	28213	1	0.00	15873	39325	12.80
5237	2	\N	\N	\N	02	2025-01-15	2025-01-15 20:00:18	6496	0	21638	3	0.00	20900	40854	15.90
5238	2	\N	\N	\N	02	2025-01-16	2025-01-16 20:00:19	4438	0	25157	3	0.00	19359	38469	11.50
5239	2	\N	\N	\N	02	2025-01-17	2025-01-17 20:00:19	3891	0	30006	10	0.00	14690	39448	9.80
5240	2	\N	\N	\N	02	2025-01-20	2025-01-20 20:00:17	27095	0	6367	16	0.00	12689	37131	72.90
5241	2	\N	\N	\N	02	2025-01-21	2025-01-21 20:00:18	30945	0	6562	20	0.00	13742	37696	82.00
5242	2	\N	\N	\N	02	2025-01-22	2025-01-22 20:00:18	27136	0	7101	6	0.00	12735	39142	69.30
5243	2	\N	\N	\N	02	2025-01-23	2025-01-23 20:00:18	14988	0	10391	9	0.00	15005	38122	39.30
5244	2	\N	\N	\N	02	2025-01-24	2025-01-24 20:00:18	19176	0	7897	4	0.00	11987	39281	48.80
5245	2	\N	\N	\N	02	2025-01-27	2025-01-27 20:00:17	5679	0	7081	4	0.00	9554	38878	14.60
5246	2	\N	\N	\N	02	2025-01-28	2025-01-28 20:00:18	10316	0	4946	30	0.00	10331	38873	26.50
5247	2	\N	\N	\N	02	2025-01-29	2025-01-29 20:00:18	7340	0	26910	9	0.00	7353	39491	18.50
5248	2	\N	\N	\N	02	2025-01-30	2025-01-30 20:00:19	3891	0	19206	5	0.00	14690	39448	9.80
5249	2	\N	\N	\N	02	2025-01-31	2025-01-31 20:00:17	10916	0	25842	22	0.00	10913	39916	27.30
5250	1	\N	\N	\N	09	2025-01-01	2025-01-01 20:00:18	7940	0	26036	21	0.00	9965	38842	20.40
5251	1	\N	\N	\N	09	2025-01-02	2025-01-02 20:00:17	4733	0	18158	1	0.00	11938	39520	11.90
5252	1	\N	\N	\N	09	2025-01-03	2025-01-03 20:00:18	26984	29	24853	14	0.00	8996	38589	69.90
5253	1	\N	\N	\N	09	2025-01-06	2025-01-06 20:00:18	19114	44	15770	1	0.00	19317	37667	50.70
5254	1	\N	\N	\N	09	2025-01-07	2025-01-07 20:00:18	18384	0	19424	1	0.00	11195	36156	50.80
5255	1	\N	\N	\N	09	2025-01-08	2025-01-08 20:00:19	20176	0	4594	1	0.00	9388	38366	52.50
5256	1	\N	\N	\N	09	2025-01-09	2025-01-09 20:00:17	10916	0	25842	5	0.00	10913	39916	27.30
5257	1	\N	\N	\N	09	2025-01-10	2025-01-10 20:00:18	15136	0	23881	0	0.00	15545	43288	34.90
5258	1	\N	\N	\N	09	2025-01-13	2025-01-13 20:00:18	10150	0	25832	5	0.00	10147	37188	27.20
5259	1	\N	\N	\N	09	2025-01-14	2025-01-14 20:00:17	15872	0	21013	12	0.00	15873	39325	40.30
5260	1	\N	\N	\N	09	2025-01-15	2025-01-15 20:00:18	20896	0	14438	12	0.00	20900	40854	51.10
5261	1	\N	\N	\N	09	2025-01-16	2025-01-16 20:00:19	4438	0	28757	2	0.00	19359	38469	11.50
5262	1	\N	\N	\N	09	2025-01-17	2025-01-17 20:00:19	3891	0	22806	4	0.00	14690	39448	9.80
5263	1	\N	\N	\N	09	2025-01-20	2025-01-20 20:00:17	5495	0	24367	16	0.00	12689	37131	14.70
5264	1	\N	\N	\N	09	2025-01-21	2025-01-21 20:00:18	12945	0	17362	11	0.00	13742	37696	34.30
3624	1	\N	\N	\N	04	2025-03-01	2025-03-01 20:00:18	7940	0	26036	8	0.00	9965	38842	25.70
3625	1	\N	\N	\N	04	2025-03-02	2025-03-02 20:00:17	11933	0	25358	13	0.00	11938	39520	30.20
3626	1	\N	\N	\N	04	2025-03-03	2025-03-03 20:00:18	8984	29	24853	4	0.00	8996	38589	23.30
3627	1	\N	\N	\N	04	2025-03-04	2025-03-04 20:00:18	12736	0	25101	6	0.00	12735	39142	32.50
3628	1	\N	\N	\N	04	2025-03-07	2025-03-07 20:00:18	14988	0	21191	9	0.00	15005	38122	39.40
3629	1	\N	\N	\N	04	2025-03-08	2025-03-08 20:00:18	19114	44	15770	7	0.00	19317	37667	51.30
3630	1	\N	\N	\N	04	2025-03-09	2025-03-09 20:00:18	11184	0	19424	18	0.00	11195	36156	31.00
3631	1	\N	\N	\N	04	2025-03-10	2025-03-10 20:00:19	9376	0	26194	4	0.00	9388	38366	24.50
3632	1	\N	\N	\N	04	2025-03-11	2025-03-11 20:00:17	10916	0	25842	13	0.00	10913	39916	27.30
3633	1	\N	\N	\N	04	2025-03-16	2025-03-16 20:00:18	15136	0	23881	4	0.00	15545	43288	35.90
3634	1	\N	\N	\N	04	2025-03-17	2025-03-17 20:00:18	11976	0	18697	5	0.00	11987	39281	30.50
3635	1	\N	\N	\N	04	2025-03-18	2025-03-18 20:00:18	10316	0	22946	8	0.00	10331	38873	26.60
3636	1	\N	\N	\N	04	2025-03-21	2025-03-21 20:00:18	10150	0	25832	8	0.00	10147	37188	27.30
3637	1	\N	\N	\N	04	2025-03-22	2025-03-22 20:00:17	15872	0	21013	10	0.00	15873	39325	40.40
3638	1	\N	\N	\N	04	2025-03-23	2025-03-23 20:00:18	20896	0	14438	9	0.00	20900	40854	51.20
3639	1	\N	\N	\N	04	2025-03-24	2025-03-24 20:00:19	18838	0	17957	9	0.00	19359	38469	50.30
3640	1	\N	\N	\N	04	2025-03-25	2025-03-25 20:00:19	14691	0	22806	5	0.00	14690	39448	37.20
3641	1	\N	\N	\N	04	2025-03-28	2025-03-28 20:00:18	7340	0	26910	9	0.00	7353	39491	18.60
3642	1	\N	\N	\N	04	2025-03-29	2025-03-29 20:00:17	9279	0	25081	10	0.00	9554	38878	24.60
3643	1	\N	\N	\N	04	2025-03-30	2025-03-30 20:00:17	12695	0	24367	6	0.00	12689	37131	34.20
3644	1	\N	\N	\N	04	2025-03-31	2025-03-31 20:00:18	12945	0	17362	11	0.00	13742	37696	36.50
3645	1	\N	\N	\N	04	2025-04-24	2025-04-24 20:00:18	12945	0	17362	11	0.00	13742	37696	36.50
3646	1	\N	\N	\N	04	2025-04-01	2025-04-01 20:00:18	7940	0	26036	28	1.00	9965	38842	33.70
3647	1	\N	\N	\N	04	2025-04-02	2025-04-02 20:00:17	11933	0	25358	2	2.00	11938	39520	30.20
3648	1	\N	\N	\N	04	2025-04-03	2025-04-03 20:00:18	8984	29	24853	3	3.00	8996	38589	27.60
3649	1	\N	\N	\N	04	2025-04-04	2025-04-04 20:00:18	12736	0	25101	4	4.00	12735	39142	22.50
3650	1	\N	\N	\N	04	2025-04-07	2025-04-07 20:00:18	14988	0	21191	5	7.00	15005	38122	39.40
3651	1	\N	\N	\N	04	2025-04-08	2025-04-08 20:00:18	19114	44	15770	6	8.00	19317	37667	51.30
3652	1	\N	\N	\N	04	2025-04-09	2025-04-09 20:00:18	11184	0	19424	7	9.00	11195	36156	31.00
3653	1	\N	\N	\N	04	2025-04-10	2025-04-10 20:00:19	9376	0	26194	8	10.00	9388	38366	24.50
3654	1	\N	\N	\N	04	2025-04-11	2025-04-11 20:00:17	10916	0	25842	9	11.00	10913	39916	27.30
3655	1	\N	\N	\N	04	2025-04-16	2025-04-16 20:00:18	15136	0	23881	10	16.00	15545	43288	35.90
3656	1	\N	\N	\N	04	2025-04-17	2025-04-17 20:00:18	11976	0	18697	11	17.00	11987	39281	30.50
3657	1	\N	\N	\N	04	2025-04-18	2025-04-18 20:00:18	10316	0	22946	12	18.00	10331	38873	26.60
3658	1	\N	\N	\N	04	2025-04-21	2025-04-21 20:00:18	10150	0	25832	13	21.00	10147	37188	27.30
3660	1	\N	\N	\N	05	2025-03-01	2025-03-01 20:00:18	7940	0	26036	8	0.00	9965	38842	25.70
3661	1	\N	\N	\N	05	2025-03-02	2025-03-02 20:00:17	11933	0	25358	13	0.00	11938	39520	30.20
3662	1	\N	\N	\N	05	2025-03-03	2025-03-03 20:00:18	8984	29	24853	4	0.00	8996	38589	23.30
3663	1	\N	\N	\N	05	2025-03-04	2025-03-04 20:00:18	12736	0	25101	6	0.00	12735	39142	32.50
3664	1	\N	\N	\N	05	2025-03-07	2025-03-07 20:00:18	14988	0	21191	9	0.00	15005	38122	39.40
3665	1	\N	\N	\N	05	2025-03-08	2025-03-08 20:00:18	19114	44	15770	7	0.00	19317	37667	51.30
3666	1	\N	\N	\N	05	2025-03-09	2025-03-09 20:00:18	11184	0	19424	18	0.00	11195	36156	31.00
3667	1	\N	\N	\N	05	2025-03-10	2025-03-10 20:00:19	9376	0	26194	4	0.00	9388	38366	24.50
3668	1	\N	\N	\N	05	2025-03-11	2025-03-11 20:00:17	10916	0	25842	13	0.00	10913	39916	27.30
3669	1	\N	\N	\N	05	2025-03-16	2025-03-16 20:00:18	15136	0	23881	4	0.00	15545	43288	35.90
3670	1	\N	\N	\N	05	2025-03-17	2025-03-17 20:00:18	11976	0	18697	5	0.00	11987	39281	30.50
3671	1	\N	\N	\N	05	2025-03-18	2025-03-18 20:00:18	10316	0	22946	8	0.00	10331	38873	26.60
3672	1	\N	\N	\N	05	2025-03-21	2025-03-21 20:00:18	10150	0	25832	8	0.00	10147	37188	27.30
3673	1	\N	\N	\N	05	2025-03-22	2025-03-22 20:00:17	15872	0	21013	10	0.00	15873	39325	40.40
3674	1	\N	\N	\N	05	2025-03-23	2025-03-23 20:00:18	20896	0	14438	9	0.00	20900	40854	51.20
3675	1	\N	\N	\N	05	2025-03-24	2025-03-24 20:00:19	18838	0	17957	9	0.00	19359	38469	50.30
3676	1	\N	\N	\N	05	2025-03-25	2025-03-25 20:00:19	14691	0	22806	5	0.00	14690	39448	37.20
3677	1	\N	\N	\N	05	2025-03-28	2025-03-28 20:00:18	7340	0	26910	9	0.00	7353	39491	18.60
3678	1	\N	\N	\N	05	2025-03-29	2025-03-29 20:00:17	9279	0	25081	10	0.00	9554	38878	24.60
3679	1	\N	\N	\N	05	2025-03-30	2025-03-30 20:00:17	12695	0	24367	6	0.00	12689	37131	34.20
3680	1	\N	\N	\N	05	2025-03-31	2025-03-31 20:00:18	12945	0	17362	11	0.00	13742	37696	36.50
3681	1	\N	\N	\N	05	2025-04-24	2025-04-24 20:00:18	12945	0	17362	11	0.00	13742	37696	36.50
3683	1	\N	\N	\N	05	2025-04-02	2025-04-02 20:00:17	11933	0	25358	2	2.00	11938	39520	30.20
3684	1	\N	\N	\N	05	2025-04-03	2025-04-03 20:00:18	8984	29	24853	3	3.00	8996	38589	27.60
5265	1	\N	\N	\N	09	2025-01-22	2025-01-22 20:00:18	5536	0	7101	2	0.00	12735	39142	14.10
5266	1	\N	\N	\N	09	2025-01-23	2025-01-23 20:00:18	14988	0	21191	2	0.00	15005	38122	39.30
5267	1	\N	\N	\N	09	2025-01-24	2025-01-24 20:00:18	11976	0	18697	15	0.00	11987	39281	30.40
5268	1	\N	\N	\N	09	2025-01-27	2025-01-27 20:00:17	27279	0	7081	10	0.00	9554	38878	70.10
5269	1	\N	\N	\N	09	2025-01-28	2025-01-28 20:00:18	28316	0	4946	20	0.00	10331	38873	72.80
5270	1	\N	\N	\N	09	2025-01-29	2025-01-29 20:00:18	7340	0	26910	13	0.00	7353	39491	18.50
5271	1	\N	\N	\N	09	2025-01-30	2025-01-30 20:00:19	14691	0	22806	15	0.00	14690	39448	37.20
5272	1	\N	\N	\N	09	2025-01-31	2025-01-31 20:00:17	10916	0	25842	13	0.00	10913	39916	27.30
3685	1	\N	\N	\N	05	2025-04-04	2025-04-04 20:00:18	12736	0	25101	4	4.00	12735	39142	22.50
3696	1	\N	\N	\N	06	2025-03-01	2025-03-01 20:00:18	7940	0	26036	8	0.00	9965	38842	25.70
3697	1	\N	\N	\N	06	2025-03-02	2025-03-02 20:00:17	11933	0	25358	13	0.00	11938	39520	30.20
3698	1	\N	\N	\N	06	2025-03-03	2025-03-03 20:00:18	8984	29	24853	4	0.00	8996	38589	23.30
3699	1	\N	\N	\N	06	2025-03-04	2025-03-04 20:00:18	12736	0	25101	6	0.00	12735	39142	32.50
3700	1	\N	\N	\N	06	2025-03-07	2025-03-07 20:00:18	14988	0	21191	9	0.00	15005	38122	39.40
3701	1	\N	\N	\N	06	2025-03-08	2025-03-08 20:00:18	19114	44	15770	7	0.00	19317	37667	51.30
3702	1	\N	\N	\N	06	2025-03-09	2025-03-09 20:00:18	11184	0	19424	18	0.00	11195	36156	31.00
3703	1	\N	\N	\N	06	2025-03-10	2025-03-10 20:00:19	9376	0	26194	4	0.00	9388	38366	24.50
3704	1	\N	\N	\N	06	2025-03-11	2025-03-11 20:00:17	10916	0	25842	13	0.00	10913	39916	27.30
3705	1	\N	\N	\N	06	2025-03-16	2025-03-16 20:00:18	15136	0	23881	4	0.00	15545	43288	35.90
3706	1	\N	\N	\N	06	2025-03-17	2025-03-17 20:00:18	11976	0	18697	5	0.00	11987	39281	30.50
3707	1	\N	\N	\N	06	2025-03-18	2025-03-18 20:00:18	10316	0	22946	8	0.00	10331	38873	26.60
3708	1	\N	\N	\N	06	2025-03-21	2025-03-21 20:00:18	10150	0	25832	8	0.00	10147	37188	27.30
3709	1	\N	\N	\N	06	2025-03-22	2025-03-22 20:00:17	15872	0	21013	10	0.00	15873	39325	40.40
3710	1	\N	\N	\N	06	2025-03-23	2025-03-23 20:00:18	20896	0	14438	9	0.00	20900	40854	51.20
3711	1	\N	\N	\N	06	2025-03-24	2025-03-24 20:00:19	18838	0	17957	9	0.00	19359	38469	50.30
3712	1	\N	\N	\N	06	2025-03-25	2025-03-25 20:00:19	14691	0	22806	5	0.00	14690	39448	37.20
3713	1	\N	\N	\N	06	2025-03-28	2025-03-28 20:00:18	7340	0	26910	9	0.00	7353	39491	18.60
3714	1	\N	\N	\N	06	2025-03-29	2025-03-29 20:00:17	9279	0	25081	10	0.00	9554	38878	24.60
3715	1	\N	\N	\N	06	2025-03-30	2025-03-30 20:00:17	12695	0	24367	6	0.00	12689	37131	34.20
3716	1	\N	\N	\N	06	2025-03-31	2025-03-31 20:00:18	12945	0	17362	11	0.00	13742	37696	36.50
5273	1	\N	\N	\N	04	2025-01-01	2025-01-01 20:00:18	7940	0	18836	12	0.00	9965	38842	20.40
5274	1	\N	\N	\N	04	2025-01-02	2025-01-02 20:00:17	11933	0	3758	13	0.00	11938	39520	30.10
5275	1	\N	\N	\N	04	2025-01-03	2025-01-03 20:00:18	8984	29	24853	12	0.00	8996	38589	23.20
5276	1	\N	\N	\N	04	2025-01-06	2025-01-06 20:00:18	19114	44	15770	11	0.00	19317	37667	50.70
5277	1	\N	\N	\N	04	2025-01-07	2025-01-07 20:00:18	11184	0	19424	11	0.00	11195	36156	30.90
5278	1	\N	\N	\N	04	2025-01-08	2025-01-08 20:00:19	9376	0	4594	11	0.00	9388	38366	24.40
5279	1	\N	\N	\N	04	2025-01-09	2025-01-09 20:00:17	10916	0	4242	13	0.00	10913	39916	27.30
5280	1	\N	\N	\N	04	2025-01-10	2025-01-10 20:00:18	15136	0	5881	9	0.00	15545	43288	34.90
5281	1	\N	\N	\N	04	2025-01-13	2025-01-13 20:00:18	28150	0	4232	8	0.00	10147	37188	75.60
5282	1	\N	\N	\N	04	2025-01-14	2025-01-14 20:00:17	15872	0	21013	8	0.00	15873	39325	40.30
5283	1	\N	\N	\N	04	2025-01-15	2025-01-15 20:00:18	20896	0	14438	8	0.00	20900	40854	51.10
5284	1	\N	\N	\N	04	2025-01-16	2025-01-16 20:00:19	29638	0	7157	7	0.00	19359	38469	77.10
5285	1	\N	\N	\N	04	2025-01-17	2025-01-17 20:00:19	14691	0	22806	7	0.00	14690	39448	37.20
5286	1	\N	\N	\N	04	2025-01-20	2025-01-20 20:00:17	23495	0	6367	6	0.00	12689	37131	63.20
5287	1	\N	\N	\N	04	2025-01-21	2025-01-21 20:00:18	23745	0	6562	6	0.00	13742	37696	63.90
5288	1	\N	\N	\N	04	2025-01-22	2025-01-22 20:00:18	12736	0	7101	6	0.00	12735	39142	32.50
5289	1	\N	\N	\N	04	2025-01-23	2025-01-23 20:00:18	14988	0	6791	5	0.00	15005	38122	39.30
5290	1	\N	\N	\N	04	2025-01-24	2025-01-24 20:00:18	11976	0	18697	5	0.00	11987	39281	30.40
5291	1	\N	\N	\N	04	2025-01-27	2025-01-27 20:00:17	9279	0	7081	4	0.00	9554	38878	23.80
5292	1	\N	\N	\N	04	2025-01-28	2025-01-28 20:00:18	10316	0	4946	4	0.00	10331	38873	26.50
5293	1	\N	\N	\N	04	2025-01-29	2025-01-29 20:00:18	7340	0	5310	3	0.00	7353	39491	18.50
5294	1	\N	\N	\N	04	2025-01-30	2025-01-30 20:00:19	29091	0	4806	2	0.00	14690	39448	73.70
5295	1	\N	\N	\N	04	2025-01-31	2025-01-31 20:00:17	10916	0	25842	1	0.00	10913	39916	27.30
3840	1	\N	\N	\N	10	2025-03-01	2025-03-01 20:00:18	7940	0	26036	8	0.00	9965	38842	25.70
3841	1	\N	\N	\N	10	2025-03-02	2025-03-02 20:00:17	11933	0	25358	13	0.00	11938	39520	30.20
3842	1	\N	\N	\N	10	2025-03-03	2025-03-03 20:00:18	8984	29	24853	4	0.00	8996	38589	23.30
3843	1	\N	\N	\N	10	2025-03-04	2025-03-04 20:00:18	12736	0	25101	6	0.00	12735	39142	32.50
3844	1	\N	\N	\N	10	2025-03-07	2025-03-07 20:00:18	14988	0	21191	9	0.00	15005	38122	39.40
3845	1	\N	\N	\N	10	2025-03-08	2025-03-08 20:00:18	19114	44	15770	7	0.00	19317	37667	51.30
3846	1	\N	\N	\N	10	2025-03-09	2025-03-09 20:00:18	11184	0	19424	18	0.00	11195	36156	31.00
3847	1	\N	\N	\N	10	2025-03-10	2025-03-10 20:00:19	9376	0	26194	4	0.00	9388	38366	24.50
3848	1	\N	\N	\N	10	2025-03-11	2025-03-11 20:00:17	10916	0	25842	13	0.00	10913	39916	27.30
3849	1	\N	\N	\N	10	2025-03-16	2025-03-16 20:00:18	15136	0	23881	4	0.00	15545	43288	35.90
3850	1	\N	\N	\N	10	2025-03-17	2025-03-17 20:00:18	11976	0	18697	5	0.00	11987	39281	30.50
3851	1	\N	\N	\N	10	2025-03-18	2025-03-18 20:00:18	10316	0	22946	8	0.00	10331	38873	26.60
3852	1	\N	\N	\N	10	2025-03-21	2025-03-21 20:00:18	10150	0	25832	8	0.00	10147	37188	27.30
3853	1	\N	\N	\N	10	2025-03-22	2025-03-22 20:00:17	15872	0	21013	10	0.00	15873	39325	40.40
3854	1	\N	\N	\N	10	2025-03-23	2025-03-23 20:00:18	20896	0	14438	9	0.00	20900	40854	51.20
3855	1	\N	\N	\N	10	2025-03-24	2025-03-24 20:00:19	18838	0	17957	9	0.00	19359	38469	50.30
3856	1	\N	\N	\N	10	2025-03-25	2025-03-25 20:00:19	14691	0	22806	5	0.00	14690	39448	37.20
3857	1	\N	\N	\N	10	2025-03-28	2025-03-28 20:00:18	7340	0	26910	9	0.00	7353	39491	18.60
3858	1	\N	\N	\N	10	2025-03-29	2025-03-29 20:00:17	9279	0	25081	10	0.00	9554	38878	24.60
3859	1	\N	\N	\N	10	2025-03-30	2025-03-30 20:00:17	12695	0	24367	6	0.00	12689	37131	34.20
3860	1	\N	\N	\N	10	2025-03-31	2025-03-31 20:00:18	12945	0	17362	11	0.00	13742	37696	36.50
3861	1	\N	\N	\N	10	2025-04-24	2025-04-24 20:00:18	12945	0	17362	11	0.00	13742	37696	36.50
3862	1	\N	\N	\N	10	2025-04-01	2025-04-01 20:00:18	7940	0	26036	28	1.00	9965	38842	33.70
3863	1	\N	\N	\N	10	2025-04-02	2025-04-02 20:00:17	11933	0	25358	2	2.00	11938	39520	30.20
3864	1	\N	\N	\N	10	2025-04-03	2025-04-03 20:00:18	8984	29	24853	3	3.00	8996	38589	27.60
3865	1	\N	\N	\N	10	2025-04-04	2025-04-04 20:00:18	12736	0	25101	4	4.00	12735	39142	22.50
3866	1	\N	\N	\N	10	2025-04-07	2025-04-07 20:00:18	14988	0	21191	5	7.00	15005	38122	39.40
3867	1	\N	\N	\N	10	2025-04-08	2025-04-08 20:00:18	19114	44	15770	6	8.00	19317	37667	51.30
3868	1	\N	\N	\N	10	2025-04-09	2025-04-09 20:00:18	11184	0	19424	7	9.00	11195	36156	31.00
3804	1	\N	\N	\N	09	2025-03-01	2025-03-01 20:00:18	7940	0	26036	8	0.00	9965	38842	25.70
3805	1	\N	\N	\N	09	2025-03-02	2025-03-02 20:00:17	11933	0	25358	13	0.00	11938	39520	30.20
3806	1	\N	\N	\N	09	2025-03-03	2025-03-03 20:00:18	8984	29	24853	4	0.00	8996	38589	23.30
3807	1	\N	\N	\N	09	2025-03-04	2025-03-04 20:00:18	12736	0	25101	6	0.00	12735	39142	32.50
3808	1	\N	\N	\N	09	2025-03-07	2025-03-07 20:00:18	14988	0	21191	9	0.00	15005	38122	39.40
3809	1	\N	\N	\N	09	2025-03-08	2025-03-08 20:00:18	19114	44	15770	7	0.00	19317	37667	51.30
3810	1	\N	\N	\N	09	2025-03-09	2025-03-09 20:00:18	11184	0	19424	18	0.00	11195	36156	31.00
3811	1	\N	\N	\N	09	2025-03-10	2025-03-10 20:00:19	9376	0	26194	4	0.00	9388	38366	24.50
3812	1	\N	\N	\N	09	2025-03-11	2025-03-11 20:00:17	10916	0	25842	13	0.00	10913	39916	27.30
3813	1	\N	\N	\N	09	2025-03-16	2025-03-16 20:00:18	15136	0	23881	4	0.00	15545	43288	35.90
3869	1	\N	\N	\N	10	2025-04-10	2025-04-10 20:00:19	9376	0	26194	8	10.00	9388	38366	24.50
3870	1	\N	\N	\N	10	2025-04-11	2025-04-11 20:00:17	10916	0	25842	9	11.00	10913	39916	27.30
3871	1	\N	\N	\N	10	2025-04-16	2025-04-16 20:00:18	15136	0	23881	10	16.00	15545	43288	35.90
3872	1	\N	\N	\N	10	2025-04-17	2025-04-17 20:00:18	11976	0	18697	11	17.00	11987	39281	30.50
3873	1	\N	\N	\N	10	2025-04-18	2025-04-18 20:00:18	10316	0	22946	12	18.00	10331	38873	26.60
3874	1	\N	\N	\N	10	2025-04-21	2025-04-21 20:00:18	10150	0	25832	13	21.00	10147	37188	27.30
3875	1	\N	\N	\N	10	2025-04-22	2025-04-22 20:00:17	15872	0	21013	14	22.00	15873	39325	40.40
3876	1	\N	\N	\N	11	2025-03-01	2025-03-01 20:00:18	7940	0	26036	8	0.00	9965	38842	25.70
3877	1	\N	\N	\N	11	2025-03-02	2025-03-02 20:00:17	11933	0	25358	13	0.00	11938	39520	30.20
3878	1	\N	\N	\N	11	2025-03-03	2025-03-03 20:00:18	8984	29	24853	4	0.00	8996	38589	23.30
3879	1	\N	\N	\N	11	2025-03-04	2025-03-04 20:00:18	12736	0	25101	6	0.00	12735	39142	32.50
3880	1	\N	\N	\N	11	2025-03-07	2025-03-07 20:00:18	14988	0	21191	9	0.00	15005	38122	39.40
3881	1	\N	\N	\N	11	2025-03-08	2025-03-08 20:00:18	19114	44	15770	7	0.00	19317	37667	51.30
3882	1	\N	\N	\N	11	2025-03-09	2025-03-09 20:00:18	11184	0	19424	18	0.00	11195	36156	31.00
3883	1	\N	\N	\N	11	2025-03-10	2025-03-10 20:00:19	9376	0	26194	4	0.00	9388	38366	24.50
3884	1	\N	\N	\N	11	2025-03-11	2025-03-11 20:00:17	10916	0	25842	13	0.00	10913	39916	27.30
3885	1	\N	\N	\N	11	2025-03-16	2025-03-16 20:00:18	15136	0	23881	4	0.00	15545	43288	35.90
3886	1	\N	\N	\N	11	2025-03-17	2025-03-17 20:00:18	11976	0	18697	5	0.00	11987	39281	30.50
3887	1	\N	\N	\N	11	2025-03-18	2025-03-18 20:00:18	10316	0	22946	8	0.00	10331	38873	26.60
3888	1	\N	\N	\N	11	2025-03-21	2025-03-21 20:00:18	10150	0	25832	8	0.00	10147	37188	27.30
3889	1	\N	\N	\N	11	2025-03-22	2025-03-22 20:00:17	15872	0	21013	10	0.00	15873	39325	40.40
3890	1	\N	\N	\N	11	2025-03-23	2025-03-23 20:00:18	20896	0	14438	9	0.00	20900	40854	51.20
3891	1	\N	\N	\N	11	2025-03-24	2025-03-24 20:00:19	18838	0	17957	9	0.00	19359	38469	50.30
3892	1	\N	\N	\N	11	2025-03-25	2025-03-25 20:00:19	14691	0	22806	5	0.00	14690	39448	37.20
3893	1	\N	\N	\N	11	2025-03-28	2025-03-28 20:00:18	7340	0	26910	9	0.00	7353	39491	18.60
3894	1	\N	\N	\N	11	2025-03-29	2025-03-29 20:00:17	9279	0	25081	10	0.00	9554	38878	24.60
3895	1	\N	\N	\N	11	2025-03-30	2025-03-30 20:00:17	12695	0	24367	6	0.00	12689	37131	34.20
3896	1	\N	\N	\N	11	2025-03-31	2025-03-31 20:00:18	12945	0	17362	11	0.00	13742	37696	36.50
3897	1	\N	\N	\N	11	2025-04-24	2025-04-24 20:00:18	12945	0	17362	11	0.00	13742	37696	36.50
3898	1	\N	\N	\N	11	2025-04-01	2025-04-01 20:00:18	7940	0	26036	28	1.00	9965	38842	33.70
3899	1	\N	\N	\N	11	2025-04-02	2025-04-02 20:00:17	11933	0	25358	2	2.00	11938	39520	30.20
3900	1	\N	\N	\N	11	2025-04-03	2025-04-03 20:00:18	8984	29	24853	3	3.00	8996	38589	27.60
3901	1	\N	\N	\N	11	2025-04-04	2025-04-04 20:00:18	12736	0	25101	4	4.00	12735	39142	22.50
3902	1	\N	\N	\N	11	2025-04-07	2025-04-07 20:00:18	14988	0	21191	5	7.00	15005	38122	39.40
3903	1	\N	\N	\N	11	2025-04-08	2025-04-08 20:00:18	19114	44	15770	6	8.00	19317	37667	51.30
3904	1	\N	\N	\N	11	2025-04-09	2025-04-09 20:00:18	11184	0	19424	7	9.00	11195	36156	31.00
3905	1	\N	\N	\N	11	2025-04-10	2025-04-10 20:00:19	9376	0	26194	8	10.00	9388	38366	24.50
3906	1	\N	\N	\N	11	2025-04-11	2025-04-11 20:00:17	10916	0	25842	9	11.00	10913	39916	27.30
3907	1	\N	\N	\N	11	2025-04-16	2025-04-16 20:00:18	15136	0	23881	10	16.00	15545	43288	35.90
3908	1	\N	\N	\N	11	2025-04-17	2025-04-17 20:00:18	11976	0	18697	11	17.00	11987	39281	30.50
3909	1	\N	\N	\N	11	2025-04-18	2025-04-18 20:00:18	10316	0	22946	12	18.00	10331	38873	26.60
3910	1	\N	\N	\N	11	2025-04-21	2025-04-21 20:00:18	10150	0	25832	13	21.00	10147	37188	27.30
3911	1	\N	\N	\N	11	2025-04-22	2025-04-22 20:00:17	15872	0	21013	14	22.00	15873	39325	40.40
3912	1	\N	\N	\N	12	2025-03-01	2025-03-01 20:00:18	7940	0	26036	8	0.00	9965	38842	25.70
3913	1	\N	\N	\N	12	2025-03-02	2025-03-02 20:00:17	11933	0	25358	13	0.00	11938	39520	30.20
3914	1	\N	\N	\N	12	2025-03-03	2025-03-03 20:00:18	8984	29	24853	4	0.00	8996	38589	23.30
3915	1	\N	\N	\N	12	2025-03-04	2025-03-04 20:00:18	12736	0	25101	6	0.00	12735	39142	32.50
3916	1	\N	\N	\N	12	2025-03-07	2025-03-07 20:00:18	14988	0	21191	9	0.00	15005	38122	39.40
3917	1	\N	\N	\N	12	2025-03-08	2025-03-08 20:00:18	19114	44	15770	7	0.00	19317	37667	51.30
3918	1	\N	\N	\N	12	2025-03-09	2025-03-09 20:00:18	11184	0	19424	18	0.00	11195	36156	31.00
3919	1	\N	\N	\N	12	2025-03-10	2025-03-10 20:00:19	9376	0	26194	4	0.00	9388	38366	24.50
3920	1	\N	\N	\N	12	2025-03-11	2025-03-11 20:00:17	10916	0	25842	13	0.00	10913	39916	27.30
3921	1	\N	\N	\N	12	2025-03-16	2025-03-16 20:00:18	15136	0	23881	4	0.00	15545	43288	35.90
3922	1	\N	\N	\N	12	2025-03-17	2025-03-17 20:00:18	11976	0	18697	5	0.00	11987	39281	30.50
3923	1	\N	\N	\N	12	2025-03-18	2025-03-18 20:00:18	10316	0	22946	8	0.00	10331	38873	26.60
3924	1	\N	\N	\N	12	2025-03-21	2025-03-21 20:00:18	10150	0	25832	8	0.00	10147	37188	27.30
3925	1	\N	\N	\N	12	2025-03-22	2025-03-22 20:00:17	15872	0	21013	10	0.00	15873	39325	40.40
3926	1	\N	\N	\N	12	2025-03-23	2025-03-23 20:00:18	20896	0	14438	9	0.00	20900	40854	51.20
3927	1	\N	\N	\N	12	2025-03-24	2025-03-24 20:00:19	18838	0	17957	9	0.00	19359	38469	50.30
3928	1	\N	\N	\N	12	2025-03-25	2025-03-25 20:00:19	14691	0	22806	5	0.00	14690	39448	37.20
3929	1	\N	\N	\N	12	2025-03-28	2025-03-28 20:00:18	7340	0	26910	9	0.00	7353	39491	18.60
3930	1	\N	\N	\N	12	2025-03-29	2025-03-29 20:00:17	9279	0	25081	10	0.00	9554	38878	24.60
3931	1	\N	\N	\N	12	2025-03-30	2025-03-30 20:00:17	12695	0	24367	6	0.00	12689	37131	34.20
3932	1	\N	\N	\N	12	2025-03-31	2025-03-31 20:00:18	12945	0	17362	11	0.00	13742	37696	36.50
3933	1	\N	\N	\N	12	2025-04-24	2025-04-24 20:00:18	12945	0	17362	11	0.00	13742	37696	36.50
3934	1	\N	\N	\N	12	2025-04-01	2025-04-01 20:00:18	7940	0	26036	28	1.00	9965	38842	33.70
3935	1	\N	\N	\N	12	2025-04-02	2025-04-02 20:00:17	11933	0	25358	2	2.00	11938	39520	30.20
3936	1	\N	\N	\N	12	2025-04-03	2025-04-03 20:00:18	8984	29	24853	3	3.00	8996	38589	27.60
3937	1	\N	\N	\N	12	2025-04-04	2025-04-04 20:00:18	12736	0	25101	4	4.00	12735	39142	22.50
3938	1	\N	\N	\N	12	2025-04-07	2025-04-07 20:00:18	14988	0	21191	5	7.00	15005	38122	39.40
3939	1	\N	\N	\N	12	2025-04-08	2025-04-08 20:00:18	19114	44	15770	6	8.00	19317	37667	51.30
3940	1	\N	\N	\N	12	2025-04-09	2025-04-09 20:00:18	11184	0	19424	7	9.00	11195	36156	31.00
3941	1	\N	\N	\N	12	2025-04-10	2025-04-10 20:00:19	9376	0	26194	8	10.00	9388	38366	24.50
3942	1	\N	\N	\N	12	2025-04-11	2025-04-11 20:00:17	10916	0	25842	9	11.00	10913	39916	27.30
3943	1	\N	\N	\N	12	2025-04-16	2025-04-16 20:00:18	15136	0	23881	10	16.00	15545	43288	35.90
3944	1	\N	\N	\N	12	2025-04-17	2025-04-17 20:00:18	11976	0	18697	11	17.00	11987	39281	30.50
3945	1	\N	\N	\N	12	2025-04-18	2025-04-18 20:00:18	10316	0	22946	12	18.00	10331	38873	26.60
3946	1	\N	\N	\N	12	2025-04-21	2025-04-21 20:00:18	10150	0	25832	13	21.00	10147	37188	27.30
3947	1	\N	\N	\N	12	2025-04-22	2025-04-22 20:00:17	15872	0	21013	14	22.00	15873	39325	40.40
3948	1	\N	\N	\N	13	2025-03-01	2025-03-01 20:00:18	7940	0	26036	8	0.00	9965	38842	25.70
3949	1	\N	\N	\N	13	2025-03-02	2025-03-02 20:00:17	11933	0	25358	13	0.00	11938	39520	30.20
3950	1	\N	\N	\N	13	2025-03-03	2025-03-03 20:00:18	8984	29	24853	4	0.00	8996	38589	23.30
3951	1	\N	\N	\N	13	2025-03-04	2025-03-04 20:00:18	12736	0	25101	6	0.00	12735	39142	32.50
3952	1	\N	\N	\N	13	2025-03-07	2025-03-07 20:00:18	14988	0	21191	9	0.00	15005	38122	39.40
3953	1	\N	\N	\N	13	2025-03-08	2025-03-08 20:00:18	19114	44	15770	7	0.00	19317	37667	51.30
3954	1	\N	\N	\N	13	2025-03-09	2025-03-09 20:00:18	11184	0	19424	18	0.00	11195	36156	31.00
3955	1	\N	\N	\N	13	2025-03-10	2025-03-10 20:00:19	9376	0	26194	4	0.00	9388	38366	24.50
3956	1	\N	\N	\N	13	2025-03-11	2025-03-11 20:00:17	10916	0	25842	13	0.00	10913	39916	27.30
3957	1	\N	\N	\N	13	2025-03-16	2025-03-16 20:00:18	15136	0	23881	4	0.00	15545	43288	35.90
3958	1	\N	\N	\N	13	2025-03-17	2025-03-17 20:00:18	11976	0	18697	5	0.00	11987	39281	30.50
3959	1	\N	\N	\N	13	2025-03-18	2025-03-18 20:00:18	10316	0	22946	8	0.00	10331	38873	26.60
3960	1	\N	\N	\N	13	2025-03-21	2025-03-21 20:00:18	10150	0	25832	8	0.00	10147	37188	27.30
3961	1	\N	\N	\N	13	2025-03-22	2025-03-22 20:00:17	15872	0	21013	10	0.00	15873	39325	40.40
3962	1	\N	\N	\N	13	2025-03-23	2025-03-23 20:00:18	20896	0	14438	9	0.00	20900	40854	51.20
3963	1	\N	\N	\N	13	2025-03-24	2025-03-24 20:00:19	18838	0	17957	9	0.00	19359	38469	50.30
3964	1	\N	\N	\N	13	2025-03-25	2025-03-25 20:00:19	14691	0	22806	5	0.00	14690	39448	37.20
3965	1	\N	\N	\N	13	2025-03-28	2025-03-28 20:00:18	7340	0	26910	9	0.00	7353	39491	18.60
3966	1	\N	\N	\N	13	2025-03-29	2025-03-29 20:00:17	9279	0	25081	10	0.00	9554	38878	24.60
3967	1	\N	\N	\N	13	2025-03-30	2025-03-30 20:00:17	12695	0	24367	6	0.00	12689	37131	34.20
3968	1	\N	\N	\N	13	2025-03-31	2025-03-31 20:00:18	12945	0	17362	11	0.00	13742	37696	36.50
3969	1	\N	\N	\N	13	2025-04-24	2025-04-24 20:00:18	12945	0	17362	11	0.00	13742	37696	36.50
3970	1	\N	\N	\N	13	2025-04-01	2025-04-01 20:00:18	7940	0	26036	28	1.00	9965	38842	33.70
3971	1	\N	\N	\N	13	2025-04-02	2025-04-02 20:00:17	11933	0	25358	2	2.00	11938	39520	30.20
3972	1	\N	\N	\N	13	2025-04-03	2025-04-03 20:00:18	8984	29	24853	3	3.00	8996	38589	27.60
3973	1	\N	\N	\N	13	2025-04-04	2025-04-04 20:00:18	12736	0	25101	4	4.00	12735	39142	22.50
3974	1	\N	\N	\N	13	2025-04-07	2025-04-07 20:00:18	14988	0	21191	5	7.00	15005	38122	39.40
3975	1	\N	\N	\N	13	2025-04-08	2025-04-08 20:00:18	19114	44	15770	6	8.00	19317	37667	51.30
3976	1	\N	\N	\N	13	2025-04-09	2025-04-09 20:00:18	11184	0	19424	7	9.00	11195	36156	31.00
3977	1	\N	\N	\N	13	2025-04-10	2025-04-10 20:00:19	9376	0	26194	8	10.00	9388	38366	24.50
3978	1	\N	\N	\N	13	2025-04-11	2025-04-11 20:00:17	10916	0	25842	9	11.00	10913	39916	27.30
3979	1	\N	\N	\N	13	2025-04-16	2025-04-16 20:00:18	15136	0	23881	10	16.00	15545	43288	35.90
3980	1	\N	\N	\N	13	2025-04-17	2025-04-17 20:00:18	11976	0	18697	11	17.00	11987	39281	30.50
3981	1	\N	\N	\N	13	2025-04-18	2025-04-18 20:00:18	10316	0	22946	12	18.00	10331	38873	26.60
3982	1	\N	\N	\N	13	2025-04-21	2025-04-21 20:00:18	10150	0	25832	13	21.00	10147	37188	27.30
3983	1	\N	\N	\N	13	2025-04-22	2025-04-22 20:00:17	15872	0	21013	14	22.00	15873	39325	40.40
3984	1	\N	\N	\N	15	2025-03-01	2025-03-01 20:00:18	7940	0	26036	8	0.00	9965	38842	25.70
3985	1	\N	\N	\N	15	2025-03-02	2025-03-02 20:00:17	11933	0	25358	13	0.00	11938	39520	30.20
3986	1	\N	\N	\N	15	2025-03-03	2025-03-03 20:00:18	8984	29	24853	4	0.00	8996	38589	23.30
3987	1	\N	\N	\N	15	2025-03-04	2025-03-04 20:00:18	12736	0	25101	6	0.00	12735	39142	32.50
3988	1	\N	\N	\N	15	2025-03-07	2025-03-07 20:00:18	14988	0	21191	9	0.00	15005	38122	39.40
3989	1	\N	\N	\N	15	2025-03-08	2025-03-08 20:00:18	19114	44	15770	7	0.00	19317	37667	51.30
3990	1	\N	\N	\N	15	2025-03-09	2025-03-09 20:00:18	11184	0	19424	18	0.00	11195	36156	31.00
3991	1	\N	\N	\N	15	2025-03-10	2025-03-10 20:00:19	9376	0	26194	4	0.00	9388	38366	24.50
3992	1	\N	\N	\N	15	2025-03-11	2025-03-11 20:00:17	10916	0	25842	13	0.00	10913	39916	27.30
3993	1	\N	\N	\N	15	2025-03-16	2025-03-16 20:00:18	15136	0	23881	4	0.00	15545	43288	35.90
3994	1	\N	\N	\N	15	2025-03-17	2025-03-17 20:00:18	11976	0	18697	5	0.00	11987	39281	30.50
3995	1	\N	\N	\N	15	2025-03-18	2025-03-18 20:00:18	10316	0	22946	8	0.00	10331	38873	26.60
3996	1	\N	\N	\N	15	2025-03-21	2025-03-21 20:00:18	10150	0	25832	8	0.00	10147	37188	27.30
3997	1	\N	\N	\N	15	2025-03-22	2025-03-22 20:00:17	15872	0	21013	10	0.00	15873	39325	40.40
3998	1	\N	\N	\N	15	2025-03-23	2025-03-23 20:00:18	20896	0	14438	9	0.00	20900	40854	51.20
3999	1	\N	\N	\N	15	2025-03-24	2025-03-24 20:00:19	18838	0	17957	9	0.00	19359	38469	50.30
4000	1	\N	\N	\N	15	2025-03-25	2025-03-25 20:00:19	14691	0	22806	5	0.00	14690	39448	37.20
4001	1	\N	\N	\N	15	2025-03-28	2025-03-28 20:00:18	7340	0	26910	9	0.00	7353	39491	18.60
4002	1	\N	\N	\N	15	2025-03-29	2025-03-29 20:00:17	9279	0	25081	10	0.00	9554	38878	24.60
4003	1	\N	\N	\N	15	2025-03-30	2025-03-30 20:00:17	12695	0	24367	6	0.00	12689	37131	34.20
4004	1	\N	\N	\N	15	2025-03-31	2025-03-31 20:00:18	12945	0	17362	11	0.00	13742	37696	36.50
4005	1	\N	\N	\N	15	2025-04-24	2025-04-24 20:00:18	12945	0	17362	11	0.00	13742	37696	36.50
4006	1	\N	\N	\N	15	2025-04-01	2025-04-01 20:00:18	7940	0	26036	28	1.00	9965	38842	33.70
4007	1	\N	\N	\N	15	2025-04-02	2025-04-02 20:00:17	11933	0	25358	2	2.00	11938	39520	30.20
4008	1	\N	\N	\N	15	2025-04-03	2025-04-03 20:00:18	8984	29	24853	3	3.00	8996	38589	27.60
4009	1	\N	\N	\N	15	2025-04-04	2025-04-04 20:00:18	12736	0	25101	4	4.00	12735	39142	22.50
4010	1	\N	\N	\N	15	2025-04-07	2025-04-07 20:00:18	14988	0	21191	5	7.00	15005	38122	39.40
4011	1	\N	\N	\N	15	2025-04-08	2025-04-08 20:00:18	19114	44	15770	6	8.00	19317	37667	51.30
4012	1	\N	\N	\N	15	2025-04-09	2025-04-09 20:00:18	11184	0	19424	7	9.00	11195	36156	31.00
4013	1	\N	\N	\N	15	2025-04-10	2025-04-10 20:00:19	9376	0	26194	8	10.00	9388	38366	24.50
4014	1	\N	\N	\N	15	2025-04-11	2025-04-11 20:00:17	10916	0	25842	9	11.00	10913	39916	27.30
4015	1	\N	\N	\N	15	2025-04-16	2025-04-16 20:00:18	15136	0	23881	10	16.00	15545	43288	35.90
4016	1	\N	\N	\N	15	2025-04-17	2025-04-17 20:00:18	11976	0	18697	11	17.00	11987	39281	30.50
4017	1	\N	\N	\N	15	2025-04-18	2025-04-18 20:00:18	10316	0	22946	12	18.00	10331	38873	26.60
4018	1	\N	\N	\N	15	2025-04-21	2025-04-21 20:00:18	10150	0	25832	13	21.00	10147	37188	27.30
4019	1	\N	\N	\N	15	2025-04-22	2025-04-22 20:00:17	15872	0	21013	14	22.00	15873	39325	40.40
4020	1	\N	\N	\N	16	2025-03-01	2025-03-01 20:00:18	7940	0	26036	8	0.00	9965	38842	25.70
4021	1	\N	\N	\N	16	2025-03-02	2025-03-02 20:00:17	11933	0	25358	13	0.00	11938	39520	30.20
4022	1	\N	\N	\N	16	2025-03-03	2025-03-03 20:00:18	8984	29	24853	4	0.00	8996	38589	23.30
4023	1	\N	\N	\N	16	2025-03-04	2025-03-04 20:00:18	12736	0	25101	6	0.00	12735	39142	32.50
4024	1	\N	\N	\N	16	2025-03-07	2025-03-07 20:00:18	14988	0	21191	9	0.00	15005	38122	39.40
4025	1	\N	\N	\N	16	2025-03-08	2025-03-08 20:00:18	19114	44	15770	7	0.00	19317	37667	51.30
4026	1	\N	\N	\N	16	2025-03-09	2025-03-09 20:00:18	11184	0	19424	18	0.00	11195	36156	31.00
4027	1	\N	\N	\N	16	2025-03-10	2025-03-10 20:00:19	9376	0	26194	4	0.00	9388	38366	24.50
4028	1	\N	\N	\N	16	2025-03-11	2025-03-11 20:00:17	10916	0	25842	13	0.00	10913	39916	27.30
4029	1	\N	\N	\N	16	2025-03-16	2025-03-16 20:00:18	15136	0	23881	4	0.00	15545	43288	35.90
4030	1	\N	\N	\N	16	2025-03-17	2025-03-17 20:00:18	11976	0	18697	5	0.00	11987	39281	30.50
4031	1	\N	\N	\N	16	2025-03-18	2025-03-18 20:00:18	10316	0	22946	8	0.00	10331	38873	26.60
4032	1	\N	\N	\N	16	2025-03-21	2025-03-21 20:00:18	10150	0	25832	8	0.00	10147	37188	27.30
4033	1	\N	\N	\N	16	2025-03-22	2025-03-22 20:00:17	15872	0	21013	10	0.00	15873	39325	40.40
4034	1	\N	\N	\N	16	2025-03-23	2025-03-23 20:00:18	20896	0	14438	9	0.00	20900	40854	51.20
4035	1	\N	\N	\N	16	2025-03-24	2025-03-24 20:00:19	18838	0	17957	9	0.00	19359	38469	50.30
4036	1	\N	\N	\N	16	2025-03-25	2025-03-25 20:00:19	14691	0	22806	5	0.00	14690	39448	37.20
4037	1	\N	\N	\N	16	2025-03-28	2025-03-28 20:00:18	7340	0	26910	9	0.00	7353	39491	18.60
4038	1	\N	\N	\N	16	2025-03-29	2025-03-29 20:00:17	9279	0	25081	10	0.00	9554	38878	24.60
4039	1	\N	\N	\N	16	2025-03-30	2025-03-30 20:00:17	12695	0	24367	6	0.00	12689	37131	34.20
4040	1	\N	\N	\N	16	2025-03-31	2025-03-31 20:00:18	12945	0	17362	11	0.00	13742	37696	36.50
4041	1	\N	\N	\N	16	2025-04-24	2025-04-24 20:00:18	12945	0	17362	11	0.00	13742	37696	36.50
4042	1	\N	\N	\N	16	2025-04-01	2025-04-01 20:00:18	7940	0	26036	28	1.00	9965	38842	33.70
4043	1	\N	\N	\N	16	2025-04-02	2025-04-02 20:00:17	11933	0	25358	2	2.00	11938	39520	30.20
4044	1	\N	\N	\N	16	2025-04-03	2025-04-03 20:00:18	8984	29	24853	3	3.00	8996	38589	27.60
4045	1	\N	\N	\N	16	2025-04-04	2025-04-04 20:00:18	12736	0	25101	4	4.00	12735	39142	22.50
4046	1	\N	\N	\N	16	2025-04-07	2025-04-07 20:00:18	14988	0	21191	5	7.00	15005	38122	39.40
4047	1	\N	\N	\N	16	2025-04-08	2025-04-08 20:00:18	19114	44	15770	6	8.00	19317	37667	51.30
4048	1	\N	\N	\N	16	2025-04-09	2025-04-09 20:00:18	11184	0	19424	7	9.00	11195	36156	31.00
4049	1	\N	\N	\N	16	2025-04-10	2025-04-10 20:00:19	9376	0	26194	8	10.00	9388	38366	24.50
4050	1	\N	\N	\N	16	2025-04-11	2025-04-11 20:00:17	10916	0	25842	9	11.00	10913	39916	27.30
4051	1	\N	\N	\N	16	2025-04-16	2025-04-16 20:00:18	15136	0	23881	10	16.00	15545	43288	35.90
4052	1	\N	\N	\N	16	2025-04-17	2025-04-17 20:00:18	11976	0	18697	11	17.00	11987	39281	30.50
4053	1	\N	\N	\N	16	2025-04-18	2025-04-18 20:00:18	10316	0	22946	12	18.00	10331	38873	26.60
4054	1	\N	\N	\N	16	2025-04-21	2025-04-21 20:00:18	10150	0	25832	13	21.00	10147	37188	27.30
4055	1	\N	\N	\N	16	2025-04-22	2025-04-22 20:00:17	15872	0	21013	14	22.00	15873	39325	40.40
4056	1	\N	\N	\N	17	2025-03-01	2025-03-01 20:00:18	7940	0	26036	8	0.00	9965	38842	25.70
4057	1	\N	\N	\N	17	2025-03-02	2025-03-02 20:00:17	11933	0	25358	13	0.00	11938	39520	30.20
4058	1	\N	\N	\N	17	2025-03-03	2025-03-03 20:00:18	8984	29	24853	4	0.00	8996	38589	23.30
4059	1	\N	\N	\N	17	2025-03-04	2025-03-04 20:00:18	12736	0	25101	6	0.00	12735	39142	32.50
4060	1	\N	\N	\N	17	2025-03-07	2025-03-07 20:00:18	14988	0	21191	9	0.00	15005	38122	39.40
4061	1	\N	\N	\N	17	2025-03-08	2025-03-08 20:00:18	19114	44	15770	7	0.00	19317	37667	51.30
4062	1	\N	\N	\N	17	2025-03-09	2025-03-09 20:00:18	11184	0	19424	18	0.00	11195	36156	31.00
4063	1	\N	\N	\N	17	2025-03-10	2025-03-10 20:00:19	9376	0	26194	4	0.00	9388	38366	24.50
4064	1	\N	\N	\N	17	2025-03-11	2025-03-11 20:00:17	10916	0	25842	13	0.00	10913	39916	27.30
4065	1	\N	\N	\N	17	2025-03-16	2025-03-16 20:00:18	15136	0	23881	4	0.00	15545	43288	35.90
4066	1	\N	\N	\N	17	2025-03-17	2025-03-17 20:00:18	11976	0	18697	5	0.00	11987	39281	30.50
4067	1	\N	\N	\N	17	2025-03-18	2025-03-18 20:00:18	10316	0	22946	8	0.00	10331	38873	26.60
4068	1	\N	\N	\N	17	2025-03-21	2025-03-21 20:00:18	10150	0	25832	8	0.00	10147	37188	27.30
4069	1	\N	\N	\N	17	2025-03-22	2025-03-22 20:00:17	15872	0	21013	10	0.00	15873	39325	40.40
4070	1	\N	\N	\N	17	2025-03-23	2025-03-23 20:00:18	20896	0	14438	9	0.00	20900	40854	51.20
4071	1	\N	\N	\N	17	2025-03-24	2025-03-24 20:00:19	18838	0	17957	9	0.00	19359	38469	50.30
4072	1	\N	\N	\N	17	2025-03-25	2025-03-25 20:00:19	14691	0	22806	5	0.00	14690	39448	37.20
4073	1	\N	\N	\N	17	2025-03-28	2025-03-28 20:00:18	7340	0	26910	9	0.00	7353	39491	18.60
4074	1	\N	\N	\N	17	2025-03-29	2025-03-29 20:00:17	9279	0	25081	10	0.00	9554	38878	24.60
4075	1	\N	\N	\N	17	2025-03-30	2025-03-30 20:00:17	12695	0	24367	6	0.00	12689	37131	34.20
4076	1	\N	\N	\N	17	2025-03-31	2025-03-31 20:00:18	12945	0	17362	11	0.00	13742	37696	36.50
4077	1	\N	\N	\N	17	2025-04-24	2025-04-24 20:00:18	12945	0	17362	11	0.00	13742	37696	36.50
4078	1	\N	\N	\N	17	2025-04-01	2025-04-01 20:00:18	7940	0	26036	28	1.00	9965	38842	33.70
4079	1	\N	\N	\N	17	2025-04-02	2025-04-02 20:00:17	11933	0	25358	2	2.00	11938	39520	30.20
4080	1	\N	\N	\N	17	2025-04-03	2025-04-03 20:00:18	8984	29	24853	3	3.00	8996	38589	27.60
4081	1	\N	\N	\N	17	2025-04-04	2025-04-04 20:00:18	12736	0	25101	4	4.00	12735	39142	22.50
4082	1	\N	\N	\N	17	2025-04-07	2025-04-07 20:00:18	14988	0	21191	5	7.00	15005	38122	39.40
4083	1	\N	\N	\N	17	2025-04-08	2025-04-08 20:00:18	19114	44	15770	6	8.00	19317	37667	51.30
4084	1	\N	\N	\N	17	2025-04-09	2025-04-09 20:00:18	11184	0	19424	7	9.00	11195	36156	31.00
4085	1	\N	\N	\N	17	2025-04-10	2025-04-10 20:00:19	9376	0	26194	8	10.00	9388	38366	24.50
4086	1	\N	\N	\N	17	2025-04-11	2025-04-11 20:00:17	10916	0	25842	9	11.00	10913	39916	27.30
4087	1	\N	\N	\N	17	2025-04-16	2025-04-16 20:00:18	15136	0	23881	10	16.00	15545	43288	35.90
4088	1	\N	\N	\N	17	2025-04-17	2025-04-17 20:00:18	11976	0	18697	11	17.00	11987	39281	30.50
4089	1	\N	\N	\N	17	2025-04-18	2025-04-18 20:00:18	10316	0	22946	12	18.00	10331	38873	26.60
4090	1	\N	\N	\N	17	2025-04-21	2025-04-21 20:00:18	10150	0	25832	13	21.00	10147	37188	27.30
4091	1	\N	\N	\N	17	2025-04-22	2025-04-22 20:00:17	15872	0	21013	14	22.00	15873	39325	40.40
4092	1	\N	\N	\N	18	2025-03-01	2025-03-01 20:00:18	7940	0	26036	8	0.00	9965	38842	25.70
4093	1	\N	\N	\N	18	2025-03-02	2025-03-02 20:00:17	11933	0	25358	13	0.00	11938	39520	30.20
4094	1	\N	\N	\N	18	2025-03-03	2025-03-03 20:00:18	8984	29	24853	4	0.00	8996	38589	23.30
4095	1	\N	\N	\N	18	2025-03-04	2025-03-04 20:00:18	12736	0	25101	6	0.00	12735	39142	32.50
4096	1	\N	\N	\N	18	2025-03-07	2025-03-07 20:00:18	14988	0	21191	9	0.00	15005	38122	39.40
4097	1	\N	\N	\N	18	2025-03-08	2025-03-08 20:00:18	19114	44	15770	7	0.00	19317	37667	51.30
4098	1	\N	\N	\N	18	2025-03-09	2025-03-09 20:00:18	11184	0	19424	18	0.00	11195	36156	31.00
4099	1	\N	\N	\N	18	2025-03-10	2025-03-10 20:00:19	9376	0	26194	4	0.00	9388	38366	24.50
4100	1	\N	\N	\N	18	2025-03-11	2025-03-11 20:00:17	10916	0	25842	13	0.00	10913	39916	27.30
4101	1	\N	\N	\N	18	2025-03-16	2025-03-16 20:00:18	15136	0	23881	4	0.00	15545	43288	35.90
4102	1	\N	\N	\N	18	2025-03-17	2025-03-17 20:00:18	11976	0	18697	5	0.00	11987	39281	30.50
4103	1	\N	\N	\N	18	2025-03-18	2025-03-18 20:00:18	10316	0	22946	8	0.00	10331	38873	26.60
4104	1	\N	\N	\N	18	2025-03-21	2025-03-21 20:00:18	10150	0	25832	8	0.00	10147	37188	27.30
4105	1	\N	\N	\N	18	2025-03-22	2025-03-22 20:00:17	15872	0	21013	10	0.00	15873	39325	40.40
4106	1	\N	\N	\N	18	2025-03-23	2025-03-23 20:00:18	20896	0	14438	9	0.00	20900	40854	51.20
4107	1	\N	\N	\N	18	2025-03-24	2025-03-24 20:00:19	18838	0	17957	9	0.00	19359	38469	50.30
4108	1	\N	\N	\N	18	2025-03-25	2025-03-25 20:00:19	14691	0	22806	5	0.00	14690	39448	37.20
4109	1	\N	\N	\N	18	2025-03-28	2025-03-28 20:00:18	7340	0	26910	9	0.00	7353	39491	18.60
4110	1	\N	\N	\N	18	2025-03-29	2025-03-29 20:00:17	9279	0	25081	10	0.00	9554	38878	24.60
4111	1	\N	\N	\N	18	2025-03-30	2025-03-30 20:00:17	12695	0	24367	6	0.00	12689	37131	34.20
4112	1	\N	\N	\N	18	2025-03-31	2025-03-31 20:00:18	12945	0	17362	11	0.00	13742	37696	36.50
4113	1	\N	\N	\N	18	2025-04-24	2025-04-24 20:00:18	12945	0	17362	11	0.00	13742	37696	36.50
4114	1	\N	\N	\N	18	2025-04-01	2025-04-01 20:00:18	7940	0	26036	28	1.00	9965	38842	33.70
4115	1	\N	\N	\N	18	2025-04-02	2025-04-02 20:00:17	11933	0	25358	2	2.00	11938	39520	30.20
4116	1	\N	\N	\N	18	2025-04-03	2025-04-03 20:00:18	8984	29	24853	3	3.00	8996	38589	27.60
4117	1	\N	\N	\N	18	2025-04-04	2025-04-04 20:00:18	12736	0	25101	4	4.00	12735	39142	22.50
4118	1	\N	\N	\N	18	2025-04-07	2025-04-07 20:00:18	14988	0	21191	5	7.00	15005	38122	39.40
4119	1	\N	\N	\N	18	2025-04-08	2025-04-08 20:00:18	19114	44	15770	6	8.00	19317	37667	51.30
4120	1	\N	\N	\N	18	2025-04-09	2025-04-09 20:00:18	11184	0	19424	7	9.00	11195	36156	31.00
4121	1	\N	\N	\N	18	2025-04-10	2025-04-10 20:00:19	9376	0	26194	8	10.00	9388	38366	24.50
4122	1	\N	\N	\N	18	2025-04-11	2025-04-11 20:00:17	10916	0	25842	9	11.00	10913	39916	27.30
4123	1	\N	\N	\N	18	2025-04-16	2025-04-16 20:00:18	15136	0	23881	10	16.00	15545	43288	35.90
4124	1	\N	\N	\N	18	2025-04-17	2025-04-17 20:00:18	11976	0	18697	11	17.00	11987	39281	30.50
4125	1	\N	\N	\N	18	2025-04-18	2025-04-18 20:00:18	10316	0	22946	12	18.00	10331	38873	26.60
4126	1	\N	\N	\N	18	2025-04-21	2025-04-21 20:00:18	10150	0	25832	13	21.00	10147	37188	27.30
4127	1	\N	\N	\N	18	2025-04-22	2025-04-22 20:00:17	15872	0	21013	14	22.00	15873	39325	40.40
4128	1	\N	\N	\N	19	2025-03-01	2025-03-01 20:00:18	7940	0	26036	8	0.00	9965	38842	25.70
4129	1	\N	\N	\N	19	2025-03-02	2025-03-02 20:00:17	11933	0	25358	13	0.00	11938	39520	30.20
4130	1	\N	\N	\N	19	2025-03-03	2025-03-03 20:00:18	8984	29	24853	4	0.00	8996	38589	23.30
4131	1	\N	\N	\N	19	2025-03-04	2025-03-04 20:00:18	12736	0	25101	6	0.00	12735	39142	32.50
4132	1	\N	\N	\N	19	2025-03-07	2025-03-07 20:00:18	14988	0	21191	9	0.00	15005	38122	39.40
4133	1	\N	\N	\N	19	2025-03-08	2025-03-08 20:00:18	19114	44	15770	7	0.00	19317	37667	51.30
4134	1	\N	\N	\N	19	2025-03-09	2025-03-09 20:00:18	11184	0	19424	18	0.00	11195	36156	31.00
4135	1	\N	\N	\N	19	2025-03-10	2025-03-10 20:00:19	9376	0	26194	4	0.00	9388	38366	24.50
4136	1	\N	\N	\N	19	2025-03-11	2025-03-11 20:00:17	10916	0	25842	13	0.00	10913	39916	27.30
4137	1	\N	\N	\N	19	2025-03-16	2025-03-16 20:00:18	15136	0	23881	4	0.00	15545	43288	35.90
4138	1	\N	\N	\N	19	2025-03-17	2025-03-17 20:00:18	11976	0	18697	5	0.00	11987	39281	30.50
4139	1	\N	\N	\N	19	2025-03-18	2025-03-18 20:00:18	10316	0	22946	8	0.00	10331	38873	26.60
4140	1	\N	\N	\N	19	2025-03-21	2025-03-21 20:00:18	10150	0	25832	8	0.00	10147	37188	27.30
4141	1	\N	\N	\N	19	2025-03-22	2025-03-22 20:00:17	15872	0	21013	10	0.00	15873	39325	40.40
4142	1	\N	\N	\N	19	2025-03-23	2025-03-23 20:00:18	20896	0	14438	9	0.00	20900	40854	51.20
4143	1	\N	\N	\N	19	2025-03-24	2025-03-24 20:00:19	18838	0	17957	9	0.00	19359	38469	50.30
4144	1	\N	\N	\N	19	2025-03-25	2025-03-25 20:00:19	14691	0	22806	5	0.00	14690	39448	37.20
4145	1	\N	\N	\N	19	2025-03-28	2025-03-28 20:00:18	7340	0	26910	9	0.00	7353	39491	18.60
4146	1	\N	\N	\N	19	2025-03-29	2025-03-29 20:00:17	9279	0	25081	10	0.00	9554	38878	24.60
4147	1	\N	\N	\N	19	2025-03-30	2025-03-30 20:00:17	12695	0	24367	6	0.00	12689	37131	34.20
4148	1	\N	\N	\N	19	2025-03-31	2025-03-31 20:00:18	12945	0	17362	11	0.00	13742	37696	36.50
4149	1	\N	\N	\N	19	2025-04-24	2025-04-24 20:00:18	12945	0	17362	11	0.00	13742	37696	36.50
4150	1	\N	\N	\N	19	2025-04-01	2025-04-01 20:00:18	7940	0	26036	28	1.00	9965	38842	33.70
4151	1	\N	\N	\N	19	2025-04-02	2025-04-02 20:00:17	11933	0	25358	2	2.00	11938	39520	30.20
4152	1	\N	\N	\N	19	2025-04-03	2025-04-03 20:00:18	8984	29	24853	3	3.00	8996	38589	27.60
4153	1	\N	\N	\N	19	2025-04-04	2025-04-04 20:00:18	12736	0	25101	4	4.00	12735	39142	22.50
4154	1	\N	\N	\N	19	2025-04-07	2025-04-07 20:00:18	14988	0	21191	5	7.00	15005	38122	39.40
4155	1	\N	\N	\N	19	2025-04-08	2025-04-08 20:00:18	19114	44	15770	6	8.00	19317	37667	51.30
4156	1	\N	\N	\N	19	2025-04-09	2025-04-09 20:00:18	11184	0	19424	7	9.00	11195	36156	31.00
4157	1	\N	\N	\N	19	2025-04-10	2025-04-10 20:00:19	9376	0	26194	8	10.00	9388	38366	24.50
4158	1	\N	\N	\N	19	2025-04-11	2025-04-11 20:00:17	10916	0	25842	9	11.00	10913	39916	27.30
4159	1	\N	\N	\N	19	2025-04-16	2025-04-16 20:00:18	15136	0	23881	10	16.00	15545	43288	35.90
4160	1	\N	\N	\N	19	2025-04-17	2025-04-17 20:00:18	11976	0	18697	11	17.00	11987	39281	30.50
4161	1	\N	\N	\N	19	2025-04-18	2025-04-18 20:00:18	10316	0	22946	12	18.00	10331	38873	26.60
4162	1	\N	\N	\N	19	2025-04-21	2025-04-21 20:00:18	10150	0	25832	13	21.00	10147	37188	27.30
4163	1	\N	\N	\N	19	2025-04-22	2025-04-22 20:00:17	15872	0	21013	14	22.00	15873	39325	40.40
4164	1	\N	\N	\N	20	2025-03-01	2025-03-01 20:00:18	7940	0	26036	8	0.00	9965	38842	25.70
4165	1	\N	\N	\N	20	2025-03-02	2025-03-02 20:00:17	11933	0	25358	13	0.00	11938	39520	30.20
4166	1	\N	\N	\N	20	2025-03-03	2025-03-03 20:00:18	8984	29	24853	4	0.00	8996	38589	23.30
4167	1	\N	\N	\N	20	2025-03-04	2025-03-04 20:00:18	12736	0	25101	6	0.00	12735	39142	32.50
4168	1	\N	\N	\N	20	2025-03-07	2025-03-07 20:00:18	14988	0	21191	9	0.00	15005	38122	39.40
4169	1	\N	\N	\N	20	2025-03-08	2025-03-08 20:00:18	19114	44	15770	7	0.00	19317	37667	51.30
4170	1	\N	\N	\N	20	2025-03-09	2025-03-09 20:00:18	11184	0	19424	18	0.00	11195	36156	31.00
4171	1	\N	\N	\N	20	2025-03-10	2025-03-10 20:00:19	9376	0	26194	4	0.00	9388	38366	24.50
4172	1	\N	\N	\N	20	2025-03-11	2025-03-11 20:00:17	10916	0	25842	13	0.00	10913	39916	27.30
4173	1	\N	\N	\N	20	2025-03-16	2025-03-16 20:00:18	15136	0	23881	4	0.00	15545	43288	35.90
4174	1	\N	\N	\N	20	2025-03-17	2025-03-17 20:00:18	11976	0	18697	5	0.00	11987	39281	30.50
4175	1	\N	\N	\N	20	2025-03-18	2025-03-18 20:00:18	10316	0	22946	8	0.00	10331	38873	26.60
4176	1	\N	\N	\N	20	2025-03-21	2025-03-21 20:00:18	10150	0	25832	8	0.00	10147	37188	27.30
4177	1	\N	\N	\N	20	2025-03-22	2025-03-22 20:00:17	15872	0	21013	10	0.00	15873	39325	40.40
4178	1	\N	\N	\N	20	2025-03-23	2025-03-23 20:00:18	20896	0	14438	9	0.00	20900	40854	51.20
4179	1	\N	\N	\N	20	2025-03-24	2025-03-24 20:00:19	18838	0	17957	9	0.00	19359	38469	50.30
4180	1	\N	\N	\N	20	2025-03-25	2025-03-25 20:00:19	14691	0	22806	5	0.00	14690	39448	37.20
4181	1	\N	\N	\N	20	2025-03-28	2025-03-28 20:00:18	7340	0	26910	9	0.00	7353	39491	18.60
4182	1	\N	\N	\N	20	2025-03-29	2025-03-29 20:00:17	9279	0	25081	10	0.00	9554	38878	24.60
4183	1	\N	\N	\N	20	2025-03-30	2025-03-30 20:00:17	12695	0	24367	6	0.00	12689	37131	34.20
4184	1	\N	\N	\N	20	2025-03-31	2025-03-31 20:00:18	12945	0	17362	11	0.00	13742	37696	36.50
4185	1	\N	\N	\N	20	2025-04-24	2025-04-24 20:00:18	12945	0	17362	11	0.00	13742	37696	36.50
4186	1	\N	\N	\N	20	2025-04-01	2025-04-01 20:00:18	7940	0	26036	28	1.00	9965	38842	33.70
4187	1	\N	\N	\N	20	2025-04-02	2025-04-02 20:00:17	11933	0	25358	2	2.00	11938	39520	30.20
4188	1	\N	\N	\N	20	2025-04-03	2025-04-03 20:00:18	8984	29	24853	3	3.00	8996	38589	27.60
4189	1	\N	\N	\N	20	2025-04-04	2025-04-04 20:00:18	12736	0	25101	4	4.00	12735	39142	22.50
4190	1	\N	\N	\N	20	2025-04-07	2025-04-07 20:00:18	14988	0	21191	5	7.00	15005	38122	39.40
4191	1	\N	\N	\N	20	2025-04-08	2025-04-08 20:00:18	19114	44	15770	6	8.00	19317	37667	51.30
4192	1	\N	\N	\N	20	2025-04-09	2025-04-09 20:00:18	11184	0	19424	7	9.00	11195	36156	31.00
4193	1	\N	\N	\N	20	2025-04-10	2025-04-10 20:00:19	9376	0	26194	8	10.00	9388	38366	24.50
4194	1	\N	\N	\N	20	2025-04-11	2025-04-11 20:00:17	10916	0	25842	9	11.00	10913	39916	27.30
4195	1	\N	\N	\N	20	2025-04-16	2025-04-16 20:00:18	15136	0	23881	10	16.00	15545	43288	35.90
4196	1	\N	\N	\N	20	2025-04-17	2025-04-17 20:00:18	11976	0	18697	11	17.00	11987	39281	30.50
4197	1	\N	\N	\N	20	2025-04-18	2025-04-18 20:00:18	10316	0	22946	12	18.00	10331	38873	26.60
4198	1	\N	\N	\N	20	2025-04-21	2025-04-21 20:00:18	10150	0	25832	13	21.00	10147	37188	27.30
4199	1	\N	\N	\N	20	2025-04-22	2025-04-22 20:00:17	15872	0	21013	14	22.00	15873	39325	40.40
4200	1	\N	\N	\N	21	2025-03-01	2025-03-01 20:00:18	7940	0	26036	8	0.00	9965	38842	25.70
4201	1	\N	\N	\N	21	2025-03-02	2025-03-02 20:00:17	11933	0	25358	13	0.00	11938	39520	30.20
4202	1	\N	\N	\N	21	2025-03-03	2025-03-03 20:00:18	8984	29	24853	4	0.00	8996	38589	23.30
4203	1	\N	\N	\N	21	2025-03-04	2025-03-04 20:00:18	12736	0	25101	6	0.00	12735	39142	32.50
4204	1	\N	\N	\N	21	2025-03-07	2025-03-07 20:00:18	14988	0	21191	9	0.00	15005	38122	39.40
4205	1	\N	\N	\N	21	2025-03-08	2025-03-08 20:00:18	19114	44	15770	7	0.00	19317	37667	51.30
4206	1	\N	\N	\N	21	2025-03-09	2025-03-09 20:00:18	11184	0	19424	18	0.00	11195	36156	31.00
4207	1	\N	\N	\N	21	2025-03-10	2025-03-10 20:00:19	9376	0	26194	4	0.00	9388	38366	24.50
4208	1	\N	\N	\N	21	2025-03-11	2025-03-11 20:00:17	10916	0	25842	13	0.00	10913	39916	27.30
4209	1	\N	\N	\N	21	2025-03-16	2025-03-16 20:00:18	15136	0	23881	4	0.00	15545	43288	35.90
4210	1	\N	\N	\N	21	2025-03-17	2025-03-17 20:00:18	11976	0	18697	5	0.00	11987	39281	30.50
4211	1	\N	\N	\N	21	2025-03-18	2025-03-18 20:00:18	10316	0	22946	8	0.00	10331	38873	26.60
4212	1	\N	\N	\N	21	2025-03-21	2025-03-21 20:00:18	10150	0	25832	8	0.00	10147	37188	27.30
4213	1	\N	\N	\N	21	2025-03-22	2025-03-22 20:00:17	15872	0	21013	10	0.00	15873	39325	40.40
4214	1	\N	\N	\N	21	2025-03-23	2025-03-23 20:00:18	20896	0	14438	9	0.00	20900	40854	51.20
4215	1	\N	\N	\N	21	2025-03-24	2025-03-24 20:00:19	18838	0	17957	9	0.00	19359	38469	50.30
4216	1	\N	\N	\N	21	2025-03-25	2025-03-25 20:00:19	14691	0	22806	5	0.00	14690	39448	37.20
4217	1	\N	\N	\N	21	2025-03-28	2025-03-28 20:00:18	7340	0	26910	9	0.00	7353	39491	18.60
4218	1	\N	\N	\N	21	2025-03-29	2025-03-29 20:00:17	9279	0	25081	10	0.00	9554	38878	24.60
4219	1	\N	\N	\N	21	2025-03-30	2025-03-30 20:00:17	12695	0	24367	6	0.00	12689	37131	34.20
4220	1	\N	\N	\N	21	2025-03-31	2025-03-31 20:00:18	12945	0	17362	11	0.00	13742	37696	36.50
4221	1	\N	\N	\N	21	2025-04-24	2025-04-24 20:00:18	12945	0	17362	11	0.00	13742	37696	36.50
4222	1	\N	\N	\N	21	2025-04-01	2025-04-01 20:00:18	7940	0	26036	28	1.00	9965	38842	33.70
4223	1	\N	\N	\N	21	2025-04-02	2025-04-02 20:00:17	11933	0	25358	2	2.00	11938	39520	30.20
4224	1	\N	\N	\N	21	2025-04-03	2025-04-03 20:00:18	8984	29	24853	3	3.00	8996	38589	27.60
4225	1	\N	\N	\N	21	2025-04-04	2025-04-04 20:00:18	12736	0	25101	4	4.00	12735	39142	22.50
4226	1	\N	\N	\N	21	2025-04-07	2025-04-07 20:00:18	14988	0	21191	5	7.00	15005	38122	39.40
4227	1	\N	\N	\N	21	2025-04-08	2025-04-08 20:00:18	19114	44	15770	6	8.00	19317	37667	51.30
4228	1	\N	\N	\N	21	2025-04-09	2025-04-09 20:00:18	11184	0	19424	7	9.00	11195	36156	31.00
4229	1	\N	\N	\N	21	2025-04-10	2025-04-10 20:00:19	9376	0	26194	8	10.00	9388	38366	24.50
4230	1	\N	\N	\N	21	2025-04-11	2025-04-11 20:00:17	10916	0	25842	9	11.00	10913	39916	27.30
4231	1	\N	\N	\N	21	2025-04-16	2025-04-16 20:00:18	15136	0	23881	10	16.00	15545	43288	35.90
4232	1	\N	\N	\N	21	2025-04-17	2025-04-17 20:00:18	11976	0	18697	11	17.00	11987	39281	30.50
4233	1	\N	\N	\N	21	2025-04-18	2025-04-18 20:00:18	10316	0	22946	12	18.00	10331	38873	26.60
4234	1	\N	\N	\N	21	2025-04-21	2025-04-21 20:00:18	10150	0	25832	13	21.00	10147	37188	27.30
4235	1	\N	\N	\N	21	2025-04-22	2025-04-22 20:00:17	15872	0	21013	14	22.00	15873	39325	40.40
4236	1	\N	\N	\N	22	2025-03-01	2025-03-01 20:00:18	7940	0	26036	8	0.00	9965	38842	25.70
4237	1	\N	\N	\N	22	2025-03-02	2025-03-02 20:00:17	11933	0	25358	13	0.00	11938	39520	30.20
4238	1	\N	\N	\N	22	2025-03-03	2025-03-03 20:00:18	8984	29	24853	4	0.00	8996	38589	23.30
4239	1	\N	\N	\N	22	2025-03-04	2025-03-04 20:00:18	12736	0	25101	6	0.00	12735	39142	32.50
4240	1	\N	\N	\N	22	2025-03-07	2025-03-07 20:00:18	14988	0	21191	9	0.00	15005	38122	39.40
4241	1	\N	\N	\N	22	2025-03-08	2025-03-08 20:00:18	19114	44	15770	7	0.00	19317	37667	51.30
4242	1	\N	\N	\N	22	2025-03-09	2025-03-09 20:00:18	11184	0	19424	18	0.00	11195	36156	31.00
4243	1	\N	\N	\N	22	2025-03-10	2025-03-10 20:00:19	9376	0	26194	4	0.00	9388	38366	24.50
4244	1	\N	\N	\N	22	2025-03-11	2025-03-11 20:00:17	10916	0	25842	13	0.00	10913	39916	27.30
4245	1	\N	\N	\N	22	2025-03-16	2025-03-16 20:00:18	15136	0	23881	4	0.00	15545	43288	35.90
4246	1	\N	\N	\N	22	2025-03-17	2025-03-17 20:00:18	11976	0	18697	5	0.00	11987	39281	30.50
4247	1	\N	\N	\N	22	2025-03-18	2025-03-18 20:00:18	10316	0	22946	8	0.00	10331	38873	26.60
4248	1	\N	\N	\N	22	2025-03-21	2025-03-21 20:00:18	10150	0	25832	8	0.00	10147	37188	27.30
4249	1	\N	\N	\N	22	2025-03-22	2025-03-22 20:00:17	15872	0	21013	10	0.00	15873	39325	40.40
4250	1	\N	\N	\N	22	2025-03-23	2025-03-23 20:00:18	20896	0	14438	9	0.00	20900	40854	51.20
4251	1	\N	\N	\N	22	2025-03-24	2025-03-24 20:00:19	18838	0	17957	9	0.00	19359	38469	50.30
4252	1	\N	\N	\N	22	2025-03-25	2025-03-25 20:00:19	14691	0	22806	5	0.00	14690	39448	37.20
4253	1	\N	\N	\N	22	2025-03-28	2025-03-28 20:00:18	7340	0	26910	9	0.00	7353	39491	18.60
4254	1	\N	\N	\N	22	2025-03-29	2025-03-29 20:00:17	9279	0	25081	10	0.00	9554	38878	24.60
4255	1	\N	\N	\N	22	2025-03-30	2025-03-30 20:00:17	12695	0	24367	6	0.00	12689	37131	34.20
4256	1	\N	\N	\N	22	2025-03-31	2025-03-31 20:00:18	12945	0	17362	11	0.00	13742	37696	36.50
4257	1	\N	\N	\N	22	2025-04-24	2025-04-24 20:00:18	12945	0	17362	11	0.00	13742	37696	36.50
4258	1	\N	\N	\N	22	2025-04-01	2025-04-01 20:00:18	7940	0	26036	28	1.00	9965	38842	33.70
4259	1	\N	\N	\N	22	2025-04-02	2025-04-02 20:00:17	11933	0	25358	2	2.00	11938	39520	30.20
4260	1	\N	\N	\N	22	2025-04-03	2025-04-03 20:00:18	8984	29	24853	3	3.00	8996	38589	27.60
4261	1	\N	\N	\N	22	2025-04-04	2025-04-04 20:00:18	12736	0	25101	4	4.00	12735	39142	22.50
4262	1	\N	\N	\N	22	2025-04-07	2025-04-07 20:00:18	14988	0	21191	5	7.00	15005	38122	39.40
4263	1	\N	\N	\N	22	2025-04-08	2025-04-08 20:00:18	19114	44	15770	6	8.00	19317	37667	51.30
4264	1	\N	\N	\N	22	2025-04-09	2025-04-09 20:00:18	11184	0	19424	7	9.00	11195	36156	31.00
4265	1	\N	\N	\N	22	2025-04-10	2025-04-10 20:00:19	9376	0	26194	8	10.00	9388	38366	24.50
4266	1	\N	\N	\N	22	2025-04-11	2025-04-11 20:00:17	10916	0	25842	9	11.00	10913	39916	27.30
4267	1	\N	\N	\N	22	2025-04-16	2025-04-16 20:00:18	15136	0	23881	10	16.00	15545	43288	35.90
4268	1	\N	\N	\N	22	2025-04-17	2025-04-17 20:00:18	11976	0	18697	11	17.00	11987	39281	30.50
4269	1	\N	\N	\N	22	2025-04-18	2025-04-18 20:00:18	10316	0	22946	12	18.00	10331	38873	26.60
4270	1	\N	\N	\N	22	2025-04-21	2025-04-21 20:00:18	10150	0	25832	13	21.00	10147	37188	27.30
4271	1	\N	\N	\N	22	2025-04-22	2025-04-22 20:00:17	15872	0	21013	14	22.00	15873	39325	40.40
3509	2	\N	\N	\N	14	2025-04-01	2025-04-01 20:00:18	7940	0	26036	8	0.00	9965	38842	25.80
3724	1	\N	\N	\N	06	2025-04-09	2025-04-09 20:00:18	11184	0	19424	7	9.00	11195	36156	31.00
3725	1	\N	\N	\N	06	2025-04-10	2025-04-10 20:00:19	9376	0	26194	8	10.00	9388	38366	24.50
3726	1	\N	\N	\N	06	2025-04-11	2025-04-11 20:00:17	10916	0	25842	9	11.00	10913	39916	27.30
3727	1	\N	\N	\N	06	2025-04-16	2025-04-16 20:00:18	15136	0	23881	10	16.00	15545	43288	35.90
3728	1	\N	\N	\N	06	2025-04-17	2025-04-17 20:00:18	11976	0	18697	11	17.00	11987	39281	30.50
3729	1	\N	\N	\N	06	2025-04-18	2025-04-18 20:00:18	10316	0	22946	12	18.00	10331	38873	26.60
3730	1	\N	\N	\N	06	2025-04-21	2025-04-21 20:00:18	10150	0	25832	13	21.00	10147	37188	27.30
3731	1	\N	\N	\N	06	2025-04-22	2025-04-22 20:00:17	15872	0	21013	14	22.00	15873	39325	40.40
3747	1	\N	\N	\N	07	2025-03-24	2025-03-24 20:00:19	18838	0	17957	9	0.00	19359	38469	50.30
3748	1	\N	\N	\N	07	2025-03-25	2025-03-25 20:00:19	14691	0	22806	5	0.00	14690	39448	37.20
3749	1	\N	\N	\N	07	2025-03-28	2025-03-28 20:00:18	7340	0	26910	9	0.00	7353	39491	18.60
3750	1	\N	\N	\N	07	2025-03-29	2025-03-29 20:00:17	9279	0	25081	10	0.00	9554	38878	24.60
3751	1	\N	\N	\N	07	2025-03-30	2025-03-30 20:00:17	12695	0	24367	6	0.00	12689	37131	34.20
3752	1	\N	\N	\N	07	2025-03-31	2025-03-31 20:00:18	12945	0	17362	11	0.00	13742	37696	36.50
3753	1	\N	\N	\N	07	2025-04-24	2025-04-24 20:00:18	12945	0	17362	11	0.00	13742	37696	36.50
3754	1	\N	\N	\N	07	2025-04-01	2025-04-01 20:00:18	7940	0	26036	28	1.00	9965	38842	33.70
3755	1	\N	\N	\N	07	2025-04-02	2025-04-02 20:00:17	11933	0	25358	2	2.00	11938	39520	30.20
3756	1	\N	\N	\N	07	2025-04-03	2025-04-03 20:00:18	8984	29	24853	3	3.00	8996	38589	27.60
3757	1	\N	\N	\N	07	2025-04-04	2025-04-04 20:00:18	12736	0	25101	4	4.00	12735	39142	22.50
3758	1	\N	\N	\N	07	2025-04-07	2025-04-07 20:00:18	14988	0	21191	5	7.00	15005	38122	39.40
3759	1	\N	\N	\N	07	2025-04-08	2025-04-08 20:00:18	19114	44	15770	6	8.00	19317	37667	51.30
4293	2	\N	\N	\N	19	2024-11-01	2024-11-01 20:00:27	0	0	0	0	0.00	0	65539	0.00
4294	2	\N	\N	\N	19	2024-11-05	2024-11-05 20:00:29	10831	943	5322	3	0.00	12573	0	13.10
4295	2	\N	\N	\N	19	2024-11-06	2024-11-06 20:00:27	16766	1427	3474	10	0.00	18775	0	20.20
4296	2	\N	\N	\N	19	2024-11-07	2024-11-07 20:00:26	16034	843	4062	10	0.00	17166	28724	59.80
4297	2	\N	\N	\N	19	2024-11-08	2024-11-08 20:00:27	4772	550	3997	2	0.00	9226	22456	41.10
4298	2	\N	\N	\N	19	2024-11-11	2024-11-11 20:00:27	17453	2729	7221	10	0.00	19462	33728	57.70
4299	2	\N	\N	\N	19	2024-11-12	2024-11-12 20:00:27	10841	1883	4310	11	0.00	12922	31032	41.60
4300	2	\N	\N	\N	19	2024-11-15	2024-11-15 20:00:28	13494	1712	3255	12	0.00	13927	29648	47.00
4301	2	\N	\N	\N	19	2024-11-18	2024-11-18 20:00:28	16463	801	2169	5	0.00	16529	24302	68.00
4302	2	\N	\N	\N	19	2024-11-19	2024-11-19 20:00:27	18524	682	7285	16	0.00	19319	30724	62.90
4303	2	\N	\N	\N	19	2024-11-20	2024-11-20 20:00:27	12293	975	2893	10	0.00	18320	31366	58.40
4304	2	\N	\N	\N	19	2024-11-21	2024-11-21 20:00:27	13071	785	6225	10	0.00	17044	32703	52.10
4305	2	\N	\N	\N	19	2024-11-22	2024-11-22 20:00:27	11735	112	4337	11	0.00	14127	31481	44.90
4306	2	\N	\N	\N	19	2024-11-25	2024-11-25 20:00:27	19011	574	9814	8	0.00	21750	35858	60.70
4307	2	\N	\N	\N	19	2024-11-26	2024-11-26 20:00:26	5228	2453	5538	4	0.00	7180	32602	22.00
4308	2	\N	\N	\N	19	2024-11-27	2024-11-27 20:00:28	12129	203	8663	7	0.00	14726	32818	44.90
4309	2	\N	\N	\N	19	2024-11-28	2024-11-28 20:00:28	9976	353	4457	4	0.00	7	7	100.00
4310	2	\N	\N	\N	19	2024-11-29	2024-11-29 20:00:27	14271	1231	5380	11	0.00	23668	38320	61.80
3760	1	\N	\N	\N	07	2025-04-09	2025-04-09 20:00:18	11184	0	19424	7	9.00	11195	36156	31.00
3761	1	\N	\N	\N	07	2025-04-10	2025-04-10 20:00:19	9376	0	26194	8	10.00	9388	38366	24.50
3762	1	\N	\N	\N	07	2025-04-11	2025-04-11 20:00:17	10916	0	25842	9	11.00	10913	39916	27.30
3763	1	\N	\N	\N	07	2025-04-16	2025-04-16 20:00:18	15136	0	23881	10	16.00	15545	43288	35.90
3764	1	\N	\N	\N	07	2025-04-17	2025-04-17 20:00:18	11976	0	18697	11	17.00	11987	39281	30.50
3765	1	\N	\N	\N	07	2025-04-18	2025-04-18 20:00:18	10316	0	22946	12	18.00	10331	38873	26.60
3766	1	\N	\N	\N	07	2025-04-21	2025-04-21 20:00:18	10150	0	25832	13	21.00	10147	37188	27.30
3767	1	\N	\N	\N	07	2025-04-22	2025-04-22 20:00:17	15872	0	21013	14	22.00	15873	39325	40.40
3781	1	\N	\N	\N	08	2025-03-22	2025-03-22 20:00:17	15872	0	21013	10	0.00	15873	39325	40.40
3782	1	\N	\N	\N	08	2025-03-23	2025-03-23 20:00:18	20896	0	14438	9	0.00	20900	40854	51.20
3783	1	\N	\N	\N	08	2025-03-24	2025-03-24 20:00:19	18838	0	17957	9	0.00	19359	38469	50.30
3784	1	\N	\N	\N	08	2025-03-25	2025-03-25 20:00:19	14691	0	22806	5	0.00	14690	39448	37.20
3785	1	\N	\N	\N	08	2025-03-28	2025-03-28 20:00:18	7340	0	26910	9	0.00	7353	39491	18.60
3786	1	\N	\N	\N	08	2025-03-29	2025-03-29 20:00:17	9279	0	25081	10	0.00	9554	38878	24.60
3787	1	\N	\N	\N	08	2025-03-30	2025-03-30 20:00:17	12695	0	24367	6	0.00	12689	37131	34.20
3788	1	\N	\N	\N	08	2025-03-31	2025-03-31 20:00:18	12945	0	17362	11	0.00	13742	37696	36.50
3789	1	\N	\N	\N	08	2025-04-24	2025-04-24 20:00:18	12945	0	17362	11	0.00	13742	37696	36.50
3790	1	\N	\N	\N	08	2025-04-01	2025-04-01 20:00:18	7940	0	26036	28	1.00	9965	38842	33.70
3791	1	\N	\N	\N	08	2025-04-02	2025-04-02 20:00:17	11933	0	25358	2	2.00	11938	39520	30.20
3792	1	\N	\N	\N	08	2025-04-03	2025-04-03 20:00:18	8984	29	24853	3	3.00	8996	38589	27.60
3793	1	\N	\N	\N	08	2025-04-04	2025-04-04 20:00:18	12736	0	25101	4	4.00	12735	39142	22.50
3794	1	\N	\N	\N	08	2025-04-07	2025-04-07 20:00:18	14988	0	21191	5	7.00	15005	38122	39.40
3795	1	\N	\N	\N	08	2025-04-08	2025-04-08 20:00:18	19114	44	15770	6	8.00	19317	37667	51.30
3796	1	\N	\N	\N	08	2025-04-09	2025-04-09 20:00:18	11184	0	19424	7	9.00	11195	36156	31.00
3797	1	\N	\N	\N	08	2025-04-10	2025-04-10 20:00:19	9376	0	26194	8	10.00	9388	38366	24.50
3798	1	\N	\N	\N	08	2025-04-11	2025-04-11 20:00:17	10916	0	25842	9	11.00	10913	39916	27.30
3799	1	\N	\N	\N	08	2025-04-16	2025-04-16 20:00:18	15136	0	23881	10	16.00	15545	43288	35.90
3800	1	\N	\N	\N	08	2025-04-17	2025-04-17 20:00:18	11976	0	18697	11	17.00	11987	39281	30.50
3801	1	\N	\N	\N	08	2025-04-18	2025-04-18 20:00:18	10316	0	22946	12	18.00	10331	38873	26.60
3802	1	\N	\N	\N	08	2025-04-21	2025-04-21 20:00:18	10150	0	25832	13	21.00	10147	37188	27.30
3803	1	\N	\N	\N	08	2025-04-22	2025-04-22 20:00:17	15872	0	21013	14	22.00	15873	39325	40.40
4586	2	\N	\N	\N	08	2025-01-31	2025-01-31 20:00:17	10916	0	25842	18	0.00	10913	39916	27.30
3814	1	\N	\N	\N	09	2025-03-17	2025-03-17 20:00:18	11976	0	18697	5	0.00	11987	39281	30.50
3815	1	\N	\N	\N	09	2025-03-18	2025-03-18 20:00:18	10316	0	22946	8	0.00	10331	38873	26.60
4909	2	\N	\N	\N	21	2025-01-01	2025-01-01 20:00:18	7940	0	26036	40	0.00	9965	38842	20.40
4910	2	\N	\N	\N	21	2025-01-02	2025-01-02 20:00:17	26333	0	3758	12	0.00	11938	39520	66.60
4911	2	\N	\N	\N	21	2025-01-03	2025-01-03 20:00:18	8984	29	24853	11	0.00	8996	38589	23.20
4912	2	\N	\N	\N	21	2025-01-06	2025-01-06 20:00:18	26314	44	8570	23	0.00	19317	37667	69.80
4913	2	\N	\N	\N	21	2025-01-07	2025-01-07 20:00:18	11184	0	19424	1	0.00	11195	36156	31.00
4914	2	\N	\N	\N	21	2025-01-08	2025-01-08 20:00:19	27376	0	4594	4	0.00	9388	38366	71.30
4915	2	\N	\N	\N	21	2025-01-09	2025-01-09 20:00:17	10916	0	25842	13	0.00	10913	39916	27.30
4916	2	\N	\N	\N	21	2025-01-10	2025-01-10 20:00:18	15136	0	23881	9	0.00	15545	43288	34.90
4917	2	\N	\N	\N	21	2025-01-13	2025-01-13 20:00:18	10150	0	25832	18	0.00	10147	37188	27.30
4918	2	\N	\N	\N	21	2025-01-14	2025-01-14 20:00:17	15872	0	21013	10	0.00	15873	39325	40.30
4919	2	\N	\N	\N	21	2025-01-15	2025-01-15 20:00:18	24496	0	7238	5	0.00	20900	40854	59.90
4920	2	\N	\N	\N	21	2025-01-16	2025-01-16 20:00:19	26038	0	7157	5	0.00	19359	38469	67.60
4921	2	\N	\N	\N	21	2025-01-17	2025-01-17 20:00:19	14691	0	22806	9	0.00	14690	39448	37.20
4922	2	\N	\N	\N	21	2025-01-20	2025-01-20 20:00:17	12695	0	24367	5	0.00	12689	37131	34.10
4923	2	\N	\N	\N	21	2025-01-21	2025-01-21 20:00:18	23745	0	6562	11	0.00	13742	37696	62.90
4924	2	\N	\N	\N	21	2025-01-22	2025-01-22 20:00:18	23536	0	7101	18	0.00	12735	39142	60.10
4925	2	\N	\N	\N	21	2025-01-23	2025-01-23 20:00:18	14988	0	21191	23	0.00	15005	38122	39.30
4926	2	\N	\N	\N	21	2025-01-24	2025-01-24 20:00:18	11976	0	18697	27	0.00	11987	39281	30.40
4927	2	\N	\N	\N	21	2025-01-27	2025-01-27 20:00:17	9279	0	25081	10	0.00	9554	38878	23.80
4928	2	\N	\N	\N	21	2025-01-28	2025-01-28 20:00:18	21116	0	4946	15	0.00	10331	38873	54.30
4929	2	\N	\N	\N	21	2025-01-29	2025-01-29 20:00:18	18140	0	5310	18	0.00	7353	39491	45.90
4930	2	\N	\N	\N	21	2025-01-30	2025-01-30 20:00:19	14691	0	22806	27	0.00	14690	39448	37.20
4931	2	\N	\N	\N	21	2025-01-31	2025-01-31 20:00:17	10916	0	25842	13	0.00	10913	39916	27.30
3816	1	\N	\N	\N	09	2025-03-21	2025-03-21 20:00:18	10150	0	25832	8	0.00	10147	37188	27.30
3817	1	\N	\N	\N	09	2025-03-22	2025-03-22 20:00:17	15872	0	21013	10	0.00	15873	39325	40.40
3818	1	\N	\N	\N	09	2025-03-23	2025-03-23 20:00:18	20896	0	14438	9	0.00	20900	40854	51.20
3819	1	\N	\N	\N	09	2025-03-24	2025-03-24 20:00:19	18838	0	17957	9	0.00	19359	38469	50.30
3820	1	\N	\N	\N	09	2025-03-25	2025-03-25 20:00:19	14691	0	22806	5	0.00	14690	39448	37.20
3821	1	\N	\N	\N	09	2025-03-28	2025-03-28 20:00:18	7340	0	26910	9	0.00	7353	39491	18.60
3822	1	\N	\N	\N	09	2025-03-29	2025-03-29 20:00:17	9279	0	25081	10	0.00	9554	38878	24.60
3823	1	\N	\N	\N	09	2025-03-30	2025-03-30 20:00:17	12695	0	24367	6	0.00	12689	37131	34.20
3824	1	\N	\N	\N	09	2025-03-31	2025-03-31 20:00:18	12945	0	17362	11	0.00	13742	37696	36.50
3825	1	\N	\N	\N	09	2025-04-24	2025-04-24 20:00:18	12945	0	17362	11	0.00	13742	37696	36.50
3826	1	\N	\N	\N	09	2025-04-01	2025-04-01 20:00:18	7940	0	26036	28	1.00	9965	38842	33.70
3827	1	\N	\N	\N	09	2025-04-02	2025-04-02 20:00:17	11933	0	25358	2	2.00	11938	39520	30.20
3828	1	\N	\N	\N	09	2025-04-03	2025-04-03 20:00:18	8984	29	24853	3	3.00	8996	38589	27.60
3829	1	\N	\N	\N	09	2025-04-04	2025-04-04 20:00:18	12736	0	25101	4	4.00	12735	39142	22.50
3830	1	\N	\N	\N	09	2025-04-07	2025-04-07 20:00:18	14988	0	21191	5	7.00	15005	38122	39.40
3831	1	\N	\N	\N	09	2025-04-08	2025-04-08 20:00:18	19114	44	15770	6	8.00	19317	37667	51.30
3832	1	\N	\N	\N	09	2025-04-09	2025-04-09 20:00:18	11184	0	19424	7	9.00	11195	36156	31.00
3833	1	\N	\N	\N	09	2025-04-10	2025-04-10 20:00:19	9376	0	26194	8	10.00	9388	38366	24.50
3834	1	\N	\N	\N	09	2025-04-11	2025-04-11 20:00:17	10916	0	25842	9	11.00	10913	39916	27.30
3835	1	\N	\N	\N	09	2025-04-16	2025-04-16 20:00:18	15136	0	23881	10	16.00	15545	43288	35.90
3836	1	\N	\N	\N	09	2025-04-17	2025-04-17 20:00:18	11976	0	18697	11	17.00	11987	39281	30.50
3837	1	\N	\N	\N	09	2025-04-18	2025-04-18 20:00:18	10316	0	22946	12	18.00	10331	38873	26.60
3838	1	\N	\N	\N	09	2025-04-21	2025-04-21 20:00:18	10150	0	25832	13	21.00	10147	37188	27.30
3839	1	\N	\N	\N	09	2025-04-22	2025-04-22 20:00:17	15872	0	21013	14	22.00	15873	39325	40.40
4449	1	\N	\N	\N	03	2025-01-01	2025-01-01 20:00:18	25940	0	4436	1	0.00	9965	38842	66.70
4450	1	\N	\N	\N	03	2025-01-02	2025-01-02 20:00:17	26333	0	3758	2	0.00	11938	39520	66.60
4451	1	\N	\N	\N	03	2025-01-03	2025-01-03 20:00:18	8984	29	24853	4	0.00	8996	38589	23.20
4452	1	\N	\N	\N	03	2025-01-06	2025-01-06 20:00:18	26314	44	8570	5	0.00	19317	37667	69.80
4453	1	\N	\N	\N	03	2025-01-07	2025-01-07 20:00:18	11184	0	19424	6	0.00	11195	36156	30.90
4454	1	\N	\N	\N	03	2025-01-08	2025-01-08 20:00:19	27376	0	4594	7	0.00	9388	38366	71.30
4455	1	\N	\N	\N	03	2025-01-09	2025-01-09 20:00:17	10916	0	25842	8	0.00	10913	39916	27.30
4456	1	\N	\N	\N	03	2025-01-10	2025-01-10 20:00:18	15136	0	23881	9	0.00	15545	43288	34.90
4457	1	\N	\N	\N	03	2025-01-13	2025-01-13 20:00:18	10150	0	25832	8	0.00	10147	37188	27.30
4458	1	\N	\N	\N	03	2025-01-14	2025-01-14 20:00:17	5072	0	21013	10	0.00	15873	39325	12.80
4459	1	\N	\N	\N	03	2025-01-15	2025-01-15 20:00:18	6496	0	14438	11	0.00	20900	40854	15.90
4460	1	\N	\N	\N	03	2025-01-16	2025-01-16 20:00:19	18838	0	17957	12	0.00	19359	38469	48.90
4461	1	\N	\N	\N	03	2025-01-17	2025-01-17 20:00:19	3891	0	22806	12	0.00	14690	39448	9.80
4462	1	\N	\N	\N	03	2025-01-20	2025-01-20 20:00:17	5495	0	31567	11	0.00	12689	37131	14.70
4463	1	\N	\N	\N	03	2025-01-21	2025-01-21 20:00:18	5745	0	24562	11	0.00	13742	37696	15.20
4464	1	\N	\N	\N	03	2025-01-22	2025-01-22 20:00:18	12736	0	25101	10	0.00	12735	39142	32.50
4465	1	\N	\N	\N	03	2025-01-23	2025-01-23 20:00:18	4188	0	31991	9	0.00	15005	38122	10.90
4466	1	\N	\N	\N	03	2025-01-24	2025-01-24 20:00:18	11976	0	18697	11	0.00	11987	39281	30.40
4467	1	\N	\N	\N	03	2025-01-27	2025-01-27 20:00:17	27279	0	7081	11	0.00	9554	38878	70.10
4468	1	\N	\N	\N	03	2025-01-28	2025-01-28 20:00:18	10316	0	22946	11	0.00	10331	38873	26.50
4469	1	\N	\N	\N	03	2025-01-29	2025-01-29 20:00:18	28940	0	5310	12	0.00	7353	39491	73.20
4470	1	\N	\N	\N	03	2025-01-30	2025-01-30 20:00:19	14691	0	22806	11	0.00	14690	39448	37.20
4471	1	\N	\N	\N	03	2025-01-31	2025-01-31 20:00:17	10916	0	25842	13	0.00	10913	39916	27.30
4495	1	\N	\N	\N	05	2025-01-01	2025-01-01 20:00:18	25940	0	4436	1	0.00	9965	38842	66.70
4496	1	\N	\N	\N	05	2025-01-02	2025-01-02 20:00:17	11933	0	25358	11	0.00	11938	39520	30.10
4497	1	\N	\N	\N	05	2025-01-03	2025-01-03 20:00:18	8984	29	24853	20	0.00	8996	38589	23.20
4498	1	\N	\N	\N	05	2025-01-06	2025-01-06 20:00:18	4714	44	26570	7	0.00	19317	37667	12.50
4499	1	\N	\N	\N	05	2025-01-07	2025-01-07 20:00:18	11184	0	26624	2	0.00	11195	36156	30.90
4500	1	\N	\N	\N	05	2025-01-08	2025-01-08 20:00:19	9376	0	26194	4	0.00	9388	38366	24.40
4501	1	\N	\N	\N	05	2025-01-09	2025-01-09 20:00:17	10916	0	25842	5	0.00	10913	39916	27.30
4502	1	\N	\N	\N	05	2025-01-10	2025-01-10 20:00:18	4336	0	27481	4	0.00	15545	43288	10.10
4503	1	\N	\N	\N	05	2025-01-13	2025-01-13 20:00:18	10150	0	11432	8	0.00	10147	37188	27.20
4504	1	\N	\N	\N	05	2025-01-14	2025-01-14 20:00:17	15872	0	21013	10	0.00	15873	39325	40.30
4505	1	\N	\N	\N	05	2025-01-15	2025-01-15 20:00:18	20896	0	14438	9	0.00	20900	40854	51.10
4506	1	\N	\N	\N	05	2025-01-16	2025-01-16 20:00:19	18838	0	17957	19	0.00	19359	38469	48.90
4507	1	\N	\N	\N	05	2025-01-17	2025-01-17 20:00:19	14691	0	4806	15	0.00	14690	39448	37.20
4508	1	\N	\N	\N	05	2025-01-20	2025-01-20 20:00:17	27095	0	6367	6	0.00	12689	37131	72.90
4509	1	\N	\N	\N	05	2025-01-21	2025-01-21 20:00:18	12945	0	17362	11	0.00	13742	37696	34.40
4510	1	\N	\N	\N	05	2025-01-22	2025-01-22 20:00:18	23536	0	7101	6	0.00	12735	39142	60.10
4511	1	\N	\N	\N	05	2025-01-23	2025-01-23 20:00:18	14988	0	21191	20	0.00	15005	38122	39.30
4512	1	\N	\N	\N	05	2025-01-24	2025-01-24 20:00:18	11976	0	18697	11	0.00	11987	39281	30.40
4513	1	\N	\N	\N	05	2025-01-27	2025-01-27 20:00:17	5679	0	25081	10	0.00	9554	38878	14.60
4518	2	\N	\N	\N	06	2025-01-01	2025-01-01 20:00:18	4340	0	26036	7	0.00	9965	38842	11.10
4519	2	\N	\N	\N	06	2025-01-02	2025-01-02 20:00:17	4733	0	25358	2	0.00	11938	39520	11.90
4520	2	\N	\N	\N	06	2025-01-03	2025-01-03 20:00:18	8984	29	24853	6	0.00	8996	38589	23.20
4521	2	\N	\N	\N	06	2025-01-06	2025-01-06 20:00:18	26314	44	4970	7	0.00	19317	37667	69.80
4522	2	\N	\N	\N	06	2025-01-07	2025-01-07 20:00:18	25584	0	5024	4	0.00	11195	36156	70.70
4523	2	\N	\N	\N	06	2025-01-08	2025-01-08 20:00:19	27376	0	4594	14	0.00	9388	38366	71.30
4524	2	\N	\N	\N	06	2025-01-09	2025-01-09 20:00:17	28916	0	4242	13	0.00	10913	39916	72.40
4525	2	\N	\N	\N	06	2025-01-10	2025-01-10 20:00:18	15136	0	23881	6	0.00	15545	43288	34.90
4526	2	\N	\N	\N	06	2025-01-13	2025-01-13 20:00:18	10150	0	4232	18	0.00	10147	37188	27.20
4527	2	\N	\N	\N	06	2025-01-14	2025-01-14 20:00:17	26672	0	6613	10	0.00	15873	39325	67.80
4528	2	\N	\N	\N	06	2025-01-15	2025-01-15 20:00:18	20896	0	14438	1	0.00	20900	40854	51.10
4529	2	\N	\N	\N	06	2025-01-16	2025-01-16 20:00:19	26038	0	7157	1	0.00	19359	38469	67.80
4530	2	\N	\N	\N	06	2025-01-17	2025-01-17 20:00:19	14691	0	4806	5	0.00	14690	39448	51.10
4531	2	\N	\N	\N	06	2025-01-20	2025-01-20 20:00:17	5495	0	24367	26	0.00	12689	37131	67.60
4532	2	\N	\N	\N	06	2025-01-21	2025-01-21 20:00:18	5745	0	17362	21	0.00	13742	37696	37.20
4533	2	\N	\N	\N	06	2025-01-22	2025-01-22 20:00:18	5536	0	25101	6	0.00	12735	39142	14.70
4534	2	\N	\N	\N	06	2025-01-23	2025-01-23 20:00:18	4188	0	21191	9	0.00	15005	38122	15.20
4535	2	\N	\N	\N	06	2025-01-24	2025-01-24 20:00:18	4776	0	18697	10	0.00	11987	39281	14.10
4536	2	\N	\N	\N	06	2025-01-27	2025-01-27 20:00:17	9279	0	25081	11	0.00	9554	38878	23.80
4537	2	\N	\N	\N	06	2025-01-28	2025-01-28 20:00:18	10316	0	4946	18	0.00	10331	38873	26.50
4538	2	\N	\N	\N	06	2025-01-29	2025-01-29 20:00:18	7340	0	5310	18	0.00	7353	39491	18.50
4539	2	\N	\N	\N	06	2025-01-30	2025-01-30 20:00:19	14691	0	22806	15	0.00	14690	39448	37.20
4540	2	\N	\N	\N	06	2025-01-31	2025-01-31 20:00:17	10916	0	25842	12	0.00	10913	39916	27.30
4541	1	\N	\N	\N	07	2025-01-01	2025-01-01 20:00:18	7940	0	26036	22	0.00	9965	38842	20.40
4542	1	\N	\N	\N	07	2025-01-02	2025-01-02 20:00:17	4733	0	3758	1	0.00	11938	39520	11.90
4543	1	\N	\N	\N	07	2025-01-03	2025-01-03 20:00:18	8984	29	24853	14	0.00	8996	38589	23.20
4544	1	\N	\N	\N	07	2025-01-06	2025-01-06 20:00:18	4714	44	15770	14	0.00	19317	37667	12.50
4545	1	\N	\N	\N	07	2025-01-07	2025-01-07 20:00:18	11184	0	19424	3	0.00	11195	36156	30.90
4546	1	\N	\N	\N	07	2025-01-08	2025-01-08 20:00:19	9376	0	4594	4	0.00	9388	38366	24.40
4547	1	\N	\N	\N	07	2025-01-09	2025-01-09 20:00:17	10916	0	4242	7	0.00	10913	39916	27.30
4548	1	\N	\N	\N	07	2025-01-10	2025-01-10 20:00:18	4336	0	23881	7	0.00	15545	43288	10.10
4549	1	\N	\N	\N	07	2025-01-13	2025-01-13 20:00:18	28150	0	4232	3	0.00	10147	37188	75.60
4550	1	\N	\N	\N	07	2025-01-14	2025-01-14 20:00:17	15872	0	21013	2	0.00	15873	39325	40.30
4551	1	\N	\N	\N	07	2025-01-15	2025-01-15 20:00:18	20896	0	14438	0	0.00	20900	40854	51.10
4552	1	\N	\N	\N	07	2025-01-16	2025-01-16 20:00:19	26038	0	7157	0	0.00	19359	38469	67.60
4553	1	\N	\N	\N	07	2025-01-17	2025-01-17 20:00:19	14691	0	22806	5	0.00	14690	39448	37.20
4554	1	\N	\N	\N	07	2025-01-20	2025-01-20 20:00:17	27095	0	6367	7	0.00	12689	37131	72.90
4555	1	\N	\N	\N	07	2025-01-21	2025-01-21 20:00:18	12945	0	17362	1	0.00	13742	37696	34.30
4556	1	\N	\N	\N	07	2025-01-22	2025-01-22 20:00:18	12736	0	25101	0	0.00	12735	39142	32.50
4557	1	\N	\N	\N	07	2025-01-23	2025-01-23 20:00:18	14988	0	21191	10	0.00	15005	38122	39.30
4558	1	\N	\N	\N	07	2025-01-24	2025-01-24 20:00:18	26376	0	4297	12	0.00	11987	39281	67.10
4559	1	\N	\N	\N	07	2025-01-27	2025-01-27 20:00:17	9279	0	7081	5	0.00	9554	38878	23.80
4560	1	\N	\N	\N	07	2025-01-28	2025-01-28 20:00:18	10316	0	22946	1	0.00	10331	38873	26.50
4561	1	\N	\N	\N	07	2025-01-29	2025-01-29 20:00:18	7340	0	5310	1	0.00	7353	39491	18.50
4562	1	\N	\N	\N	07	2025-01-30	2025-01-30 20:00:19	14691	0	22806	15	0.00	14690	39448	37.20
4563	1	\N	\N	\N	07	2025-01-31	2025-01-31 20:00:17	3716	0	25842	16	0.00	10913	39916	9.30
4564	2	\N	\N	\N	08	2025-01-01	2025-01-01 20:00:18	25940	0	836	1	0.00	9965	38842	66.70
4565	2	\N	\N	\N	08	2025-01-02	2025-01-02 20:00:17	29933	0	158	1	0.00	11938	39520	75.70
4610	1	\N	\N	\N	10	2025-01-01	2025-01-01 20:00:18	7940	0	4436	5	0.00	9965	38842	20.40
4611	1	\N	\N	\N	10	2025-01-02	2025-01-02 20:00:17	4733	0	3758	6	0.00	11938	39520	11.90
4612	1	\N	\N	\N	10	2025-01-03	2025-01-03 20:00:18	8984	29	6853	2	0.00	8996	38589	23.20
4613	1	\N	\N	\N	10	2025-01-06	2025-01-06 20:00:18	4714	44	4970	17	0.00	19317	37667	12.50
4614	1	\N	\N	\N	10	2025-01-07	2025-01-07 20:00:18	11184	0	19424	18	0.00	11195	36156	30.90
4615	1	\N	\N	\N	10	2025-01-08	2025-01-08 20:00:19	9376	0	26194	14	0.00	9388	38366	24.40
4616	1	\N	\N	\N	10	2025-01-09	2025-01-09 20:00:17	10916	0	25842	13	0.00	10913	39916	27.30
4617	1	\N	\N	\N	10	2025-01-10	2025-01-10 20:00:18	15136	0	23881	14	0.00	15545	43288	34.90
4566	2	\N	\N	\N	08	2025-01-03	2025-01-03 20:00:18	26984	29	3253	14	0.00	8996	38589	69.90
4567	2	\N	\N	\N	08	2025-01-06	2025-01-06 20:00:18	4714	44	15770	1	0.00	19317	37667	12.50
4568	2	\N	\N	\N	08	2025-01-07	2025-01-07 20:00:18	11184	0	19424	2	0.00	11195	36156	30.90
4569	2	\N	\N	\N	08	2025-01-08	2025-01-08 20:00:19	20176	0	26194	3	0.00	9388	38366	52.50
4570	2	\N	\N	\N	08	2025-01-09	2025-01-09 20:00:17	10916	0	4242	13	0.00	10913	39916	27.30
4571	2	\N	\N	\N	08	2025-01-10	2025-01-10 20:00:18	18736	0	23881	5	0.00	15545	43288	43.20
4572	2	\N	\N	\N	08	2025-01-13	2025-01-13 20:00:18	10150	0	25832	14	0.00	10147	37188	27.20
4573	2	\N	\N	\N	08	2025-01-14	2025-01-14 20:00:17	15872	0	21013	11	0.00	15873	39325	40.30
4574	2	\N	\N	\N	08	2025-01-15	2025-01-15 20:00:18	10096	0	18038	16	0.00	20900	40854	24.70
4575	2	\N	\N	\N	08	2025-01-16	2025-01-16 20:00:19	8038	0	7157	5	0.00	19359	38469	20.80
4576	2	\N	\N	\N	08	2025-01-17	2025-01-17 20:00:19	14691	0	22806	6	0.00	14690	39448	37.20
4577	2	\N	\N	\N	08	2025-01-20	2025-01-20 20:00:17	12695	0	24367	13	0.00	12689	37131	34.10
4578	2	\N	\N	\N	08	2025-01-21	2025-01-21 20:00:18	12945	0	17362	1	0.00	13742	37696	34.30
4579	2	\N	\N	\N	08	2025-01-22	2025-01-22 20:00:18	5536	0	25101	4	0.00	12735	39142	14.10
4580	2	\N	\N	\N	08	2025-01-23	2025-01-23 20:00:18	4188	0	21191	2	0.00	15005	38122	10.90
4581	2	\N	\N	\N	08	2025-01-24	2025-01-24 20:00:18	11976	0	18697	2	0.00	11987	39281	30.40
4582	2	\N	\N	\N	08	2025-01-27	2025-01-27 20:00:17	5679	0	21481	10	0.00	9554	38878	14.60
4583	2	\N	\N	\N	08	2025-01-28	2025-01-28 20:00:18	10316	0	19346	11	0.00	10331	38873	26.50
4584	2	\N	\N	\N	08	2025-01-29	2025-01-29 20:00:18	7340	0	26910	11	0.00	7353	39491	18.50
4585	2	\N	\N	\N	08	2025-01-30	2025-01-30 20:00:19	3891	0	22806	5	0.00	14690	39448	9.80
4618	1	\N	\N	\N	10	2025-01-13	2025-01-13 20:00:18	10150	0	4232	4	0.00	10147	37188	27.30
4619	1	\N	\N	\N	10	2025-01-14	2025-01-14 20:00:17	15872	0	21013	10	0.00	15873	39325	40.30
4620	1	\N	\N	\N	10	2025-01-15	2025-01-15 20:00:18	20896	0	14438	5	0.00	20900	40854	51.10
4621	1	\N	\N	\N	10	2025-01-16	2025-01-16 20:00:19	4438	0	17957	15	0.00	19359	38469	11.50
4622	1	\N	\N	\N	10	2025-01-17	2025-01-17 20:00:19	14691	0	22806	3	0.00	14690	39448	37.20
4623	1	\N	\N	\N	10	2025-01-20	2025-01-20 20:00:17	12695	0	24367	20	0.00	12689	37131	34.10
4624	1	\N	\N	\N	10	2025-01-21	2025-01-21 20:00:18	12945	0	17362	11	0.00	13742	37696	34.30
4625	1	\N	\N	\N	10	2025-01-22	2025-01-22 20:00:18	5536	0	25101	18	0.00	12735	39142	14.10
4626	1	\N	\N	\N	10	2025-01-23	2025-01-23 20:00:18	4188	0	6791	9	0.00	15005	38122	10.90
4627	1	\N	\N	\N	10	2025-01-24	2025-01-24 20:00:18	11976	0	4297	6	0.00	11987	39281	30.40
4628	1	\N	\N	\N	10	2025-01-27	2025-01-27 20:00:17	9279	0	25081	4	0.00	9554	38878	23.80
4629	1	\N	\N	\N	10	2025-01-28	2025-01-28 20:00:18	10316	0	22946	4	0.00	10331	38873	26.50
4630	1	\N	\N	\N	10	2025-01-29	2025-01-29 20:00:18	7340	0	5310	4	0.00	7353	39491	18.50
4631	1	\N	\N	\N	10	2025-01-30	2025-01-30 20:00:19	3891	0	22806	7	0.00	14690	39448	9.80
4632	1	\N	\N	\N	10	2025-01-31	2025-01-31 20:00:17	10916	0	4242	8	0.00	10913	39916	27.30
4633	2	\N	\N	\N	11	2025-01-01	2025-01-01 20:00:18	7940	0	18836	20	0.00	9965	38842	20.40
4634	2	\N	\N	\N	11	2025-01-02	2025-01-02 20:00:17	19133	0	18158	13	0.00	11938	39520	48.40
4635	2	\N	\N	\N	11	2025-01-03	2025-01-03 20:00:18	5384	29	28453	16	0.00	8996	38589	13.90
4636	2	\N	\N	\N	11	2025-01-06	2025-01-06 20:00:18	19114	44	15770	13	0.00	19317	37667	50.70
4637	2	\N	\N	\N	11	2025-01-07	2025-01-07 20:00:18	11184	0	19424	18	0.00	11195	36156	30.90
4638	2	\N	\N	\N	11	2025-01-08	2025-01-08 20:00:19	27376	0	4594	14	0.00	9388	38366	71.30
4639	2	\N	\N	\N	11	2025-01-09	2025-01-09 20:00:17	25316	0	4242	13	0.00	10913	39916	63.40
4640	2	\N	\N	\N	11	2025-01-10	2025-01-10 20:00:18	15136	0	23881	15	0.00	15545	43288	34.90
4641	2	\N	\N	\N	11	2025-01-13	2025-01-13 20:00:18	28150	0	4232	8	0.00	10147	37188	75.60
4642	2	\N	\N	\N	11	2025-01-14	2025-01-14 20:00:17	15872	0	21013	15	0.00	15873	39325	40.30
4643	2	\N	\N	\N	11	2025-01-15	2025-01-15 20:00:18	20896	0	14438	15	0.00	20900	40854	51.10
4644	2	\N	\N	\N	11	2025-01-16	2025-01-16 20:00:19	18838	0	17957	18	0.00	19359	38469	48.90
4645	2	\N	\N	\N	11	2025-01-17	2025-01-17 20:00:19	29091	0	4806	20	0.00	14690	39448	73.70
4646	2	\N	\N	\N	11	2025-01-20	2025-01-20 20:00:17	12695	0	24367	10	0.00	12689	37131	34.10
4647	2	\N	\N	\N	11	2025-01-21	2025-01-21 20:00:18	27345	0	6562	21	0.00	13742	37696	72.50
4648	2	\N	\N	\N	11	2025-01-22	2025-01-22 20:00:18	12736	0	7101	22	0.00	12735	39142	32.50
4649	2	\N	\N	\N	11	2025-01-23	2025-01-23 20:00:18	14988	0	21191	16	0.00	15005	38122	39.30
4650	2	\N	\N	\N	11	2025-01-24	2025-01-24 20:00:18	11976	0	18697	18	0.00	11987	39281	30.40
4651	2	\N	\N	\N	11	2025-01-27	2025-01-27 20:00:17	9279	0	25081	18	0.00	9554	38878	23.80
4652	2	\N	\N	\N	11	2025-01-28	2025-01-28 20:00:18	10316	0	22946	15	0.00	10331	38873	26.50
4653	2	\N	\N	\N	11	2025-01-29	2025-01-29 20:00:18	7340	0	5310	15	0.00	7353	39491	18.50
4654	2	\N	\N	\N	11	2025-01-30	2025-01-30 20:00:19	3891	0	22806	18	0.00	14690	39448	9.80
4655	2	\N	\N	\N	11	2025-01-31	2025-01-31 20:00:17	3716	0	25842	11	0.00	10913	39916	9.30
4656	1	\N	\N	\N	12	2025-01-01	2025-01-01 20:00:18	18740	0	15236	3	0.00	9965	38842	48.20
4657	1	\N	\N	\N	12	2025-01-02	2025-01-02 20:00:17	19133	0	14558	4	0.00	11938	39520	48.40
4658	1	\N	\N	\N	12	2025-01-03	2025-01-03 20:00:18	8984	29	21253	12	0.00	8996	38589	23.20
4659	1	\N	\N	\N	12	2025-01-06	2025-01-06 20:00:18	19114	44	15770	7	0.00	19317	37667	50.70
4660	1	\N	\N	\N	12	2025-01-07	2025-01-07 20:00:18	18384	0	19424	8	0.00	11195	36156	50.80
4661	1	\N	\N	\N	12	2025-01-08	2025-01-08 20:00:19	20176	0	15394	8	0.00	9388	38366	52.50
4662	1	\N	\N	\N	12	2025-01-09	2025-01-09 20:00:17	10916	0	25842	13	0.00	10913	39916	27.30
4663	1	\N	\N	\N	12	2025-01-10	2025-01-10 20:00:18	15136	0	23881	8	0.00	15545	43288	34.90
4664	1	\N	\N	\N	12	2025-01-13	2025-01-13 20:00:18	20950	0	15032	2	0.00	10147	37188	56.30
4665	1	\N	\N	\N	12	2025-01-14	2025-01-14 20:00:17	15872	0	21013	12	0.00	15873	39325	10.30
4666	1	\N	\N	\N	12	2025-01-15	2025-01-15 20:00:18	20896	0	14438	12	0.00	20900	40854	51.10
4667	1	\N	\N	\N	12	2025-01-16	2025-01-16 20:00:19	18838	0	17957	1	0.00	19359	38469	48.90
4668	1	\N	\N	\N	12	2025-01-17	2025-01-17 20:00:19	14691	0	22806	1	0.00	14690	39448	37.20
4669	1	\N	\N	\N	12	2025-01-20	2025-01-20 20:00:17	12695	0	20767	6	0.00	12689	37131	34.10
4670	1	\N	\N	\N	12	2025-01-21	2025-01-21 20:00:18	12945	0	17362	8	0.00	13742	37696	34.30
4671	1	\N	\N	\N	12	2025-01-22	2025-01-22 20:00:18	12736	0	25101	8	0.00	12735	39142	32.50
4672	1	\N	\N	\N	12	2025-01-23	2025-01-23 20:00:18	4188	0	28391	9	0.00	15005	38122	10.90
4673	1	\N	\N	\N	12	2025-01-24	2025-01-24 20:00:18	11976	0	18697	15	0.00	11987	39281	30.40
4674	1	\N	\N	\N	12	2025-01-27	2025-01-27 20:00:17	9279	0	25081	14	0.00	9554	38878	23.80
4675	1	\N	\N	\N	12	2025-01-28	2025-01-28 20:00:18	13916	0	12146	6	0.00	10331	38873	35.70
4676	1	\N	\N	\N	12	2025-01-29	2025-01-29 20:00:18	10940	0	12510	14	0.00	7353	39491	27.70
4677	1	\N	\N	\N	12	2025-01-30	2025-01-30 20:00:19	11091	0	22806	17	0.00	14690	39448	28.10
4678	1	\N	\N	\N	12	2025-01-31	2025-01-31 20:00:17	10916	0	25842	13	0.00	10913	39916	27.30
4679	1	\N	\N	\N	13	2025-01-01	2025-01-01 20:00:18	29540	0	4436	22	0.00	9965	38842	76.00
4680	1	\N	\N	\N	13	2025-01-02	2025-01-02 20:00:17	29933	0	158	18	0.00	11938	39520	75.70
4681	1	\N	\N	\N	13	2025-01-03	2025-01-03 20:00:18	30584	29	4213	1	0.00	8996	38589	79.20
4682	1	\N	\N	\N	13	2025-01-06	2025-01-06 20:00:18	19114	44	15770	1	0.00	19317	37667	50.70
4683	1	\N	\N	\N	13	2025-01-07	2025-01-07 20:00:18	3984	0	19424	2	0.00	11195	36156	11.00
4684	1	\N	\N	\N	13	2025-01-08	2025-01-08 20:00:19	9376	0	4594	0	0.00	9388	38366	24.40
4685	1	\N	\N	\N	13	2025-01-09	2025-01-09 20:00:17	3716	0	25842	0	0.00	10913	39916	9.30
4686	1	\N	\N	\N	13	2025-01-10	2025-01-10 20:00:18	15136	0	23881	5	0.00	15545	43288	34.90
4687	1	\N	\N	\N	13	2025-01-13	2025-01-13 20:00:18	10150	0	25832	0	0.00	10147	37188	27.20
4688	1	\N	\N	\N	13	2025-01-14	2025-01-14 20:00:17	15872	0	21013	4	0.00	15873	39325	40.30
4689	1	\N	\N	\N	13	2025-01-15	2025-01-15 20:00:18	2896	0	28838	12	0.00	20900	40854	7.00
4690	1	\N	\N	\N	13	2025-01-16	2025-01-16 20:00:19	18838	0	17957	17	0.00	19359	38469	48.90
4691	1	\N	\N	\N	13	2025-01-17	2025-01-17 20:00:19	7491	0	26406	12	0.00	14690	39448	18.90
4692	1	\N	\N	\N	13	2025-01-20	2025-01-20 20:00:17	12695	0	24367	23	0.00	12689	37131	34.10
4693	1	\N	\N	\N	13	2025-01-21	2025-01-21 20:00:18	30945	0	6562	21	0.00	13742	37696	82.00
4694	1	\N	\N	\N	13	2025-01-22	2025-01-22 20:00:18	12736	0	25101	18	0.00	12735	39142	32.50
4695	1	\N	\N	\N	13	2025-01-23	2025-01-23 20:00:18	29388	0	6791	11	0.00	15005	38122	77.00
4696	1	\N	\N	\N	13	2025-01-24	2025-01-24 20:00:18	11976	0	4297	6	0.00	11987	39281	30.40
4697	1	\N	\N	\N	13	2025-01-27	2025-01-27 20:00:17	5679	0	32281	5	0.00	9554	38878	14.60
4698	1	\N	\N	\N	13	2025-01-28	2025-01-28 20:00:18	10316	0	22946	7	0.00	10331	38873	26.50
4699	1	\N	\N	\N	13	2025-01-29	2025-01-29 20:00:18	7340	0	26910	8	0.00	7353	39491	18.50
4700	1	\N	\N	\N	13	2025-01-30	2025-01-30 20:00:19	3891	0	22806	12	0.00	14690	39448	9.80
4701	1	\N	\N	\N	13	2025-01-31	2025-01-31 20:00:17	3716	0	25842	13	0.00	10913	39916	9.30
4702	1	\N	\N	\N	14	2025-01-01	2025-01-01 20:00:18	18740	0	15236	12	0.00	9965	38842	48.20
4703	1	\N	\N	\N	14	2025-01-02	2025-01-02 20:00:17	19133	0	10958	13	0.00	11938	39520	48.40
4704	1	\N	\N	\N	14	2025-01-03	2025-01-03 20:00:18	19784	29	14053	15	0.00	8996	38589	51.20
4705	1	\N	\N	\N	14	2025-01-06	2025-01-06 20:00:18	4714	44	15770	12	0.00	19317	37667	12.50
4706	1	\N	\N	\N	14	2025-01-07	2025-01-07 20:00:18	11184	0	19424	18	0.00	11195	36156	30.90
4707	1	\N	\N	\N	14	2025-01-08	2025-01-08 20:00:19	9376	0	4594	11	0.00	9388	38366	24.40
4708	1	\N	\N	\N	14	2025-01-09	2025-01-09 20:00:17	10916	0	25842	13	0.00	10913	39916	27.30
4709	1	\N	\N	\N	14	2025-01-10	2025-01-10 20:00:18	4336	0	23881	19	0.00	15545	43288	10.00
4710	1	\N	\N	\N	14	2025-01-13	2025-01-13 20:00:18	10150	0	4232	1	0.00	10147	37188	27.20
4711	1	\N	\N	\N	14	2025-01-14	2025-01-14 20:00:17	15872	0	21013	20	0.00	15873	39325	40.30
4712	1	\N	\N	\N	14	2025-01-15	2025-01-15 20:00:18	6496	0	14438	13	0.00	20900	40854	16.00
4713	1	\N	\N	\N	14	2025-01-16	2025-01-16 20:00:19	4438	0	17957	12	0.00	19359	38469	11.50
4714	1	\N	\N	\N	14	2025-01-17	2025-01-17 20:00:19	14691	0	22806	7	0.00	14690	39448	37.20
4715	1	\N	\N	\N	14	2025-01-20	2025-01-20 20:00:17	12695	0	24367	8	0.00	12689	37131	34.10
4716	1	\N	\N	\N	14	2025-01-21	2025-01-21 20:00:18	12945	0	17362	7	0.00	13742	37696	34.30
4717	1	\N	\N	\N	14	2025-01-22	2025-01-22 20:00:18	12736	0	7101	12	0.00	12735	39142	32.50
4718	1	\N	\N	\N	14	2025-01-23	2025-01-23 20:00:18	4188	0	21191	16	0.00	15005	38122	10.90
4719	1	\N	\N	\N	14	2025-01-24	2025-01-24 20:00:18	11976	0	18697	15	0.00	11987	39281	30.40
4720	1	\N	\N	\N	14	2025-01-27	2025-01-27 20:00:17	9279	0	25081	16	0.00	9554	38878	23.80
4721	1	\N	\N	\N	14	2025-01-28	2025-01-28 20:00:18	10316	0	22946	16	0.00	10331	38873	26.50
4722	1	\N	\N	\N	14	2025-01-29	2025-01-29 20:00:18	7340	0	26910	12	0.00	7353	39491	18.50
4723	1	\N	\N	\N	14	2025-01-30	2025-01-30 20:00:19	3891	0	22806	12	0.00	14690	39448	9.80
4724	1	\N	\N	\N	14	2025-01-31	2025-01-31 20:00:17	10916	0	4242	13	0.00	10913	39916	27.30
4725	1	\N	\N	\N	15	2025-01-01	2025-01-01 20:00:18	7940	0	4436	3	0.00	9965	38842	20.40
4726	1	\N	\N	\N	15	2025-01-02	2025-01-02 20:00:17	11933	0	3758	8	0.00	11938	39520	30.10
4727	1	\N	\N	\N	15	2025-01-03	2025-01-03 20:00:18	5384	29	24853	9	0.00	8996	38589	13.90
4728	1	\N	\N	\N	15	2025-01-06	2025-01-06 20:00:18	19114	44	15770	2	0.00	19317	37667	50.70
4729	1	\N	\N	\N	15	2025-01-07	2025-01-07 20:00:18	11184	0	5024	0	0.00	11195	36156	31.00
4730	1	\N	\N	\N	15	2025-01-08	2025-01-08 20:00:19	9376	0	26194	4	0.00	9388	38366	24.40
4731	1	\N	\N	\N	15	2025-01-09	2025-01-09 20:00:17	10916	0	4242	13	0.00	10913	39916	27.30
4732	1	\N	\N	\N	15	2025-01-10	2025-01-10 20:00:18	4336	0	23881	15	0.00	15545	43288	10.00
4733	1	\N	\N	\N	15	2025-01-13	2025-01-13 20:00:18	10150	0	25832	18	0.00	10147	37188	27.30
4734	1	\N	\N	\N	15	2025-01-14	2025-01-14 20:00:17	15872	0	21013	10	0.00	15873	39325	40.40
4735	1	\N	\N	\N	15	2025-01-15	2025-01-15 20:00:18	6496	0	14438	0	0.00	20900	40854	16.00
4736	1	\N	\N	\N	15	2025-01-16	2025-01-16 20:00:19	18838	0	17957	9	0.00	19359	38469	49.00
4737	1	\N	\N	\N	15	2025-01-17	2025-01-17 20:00:19	14691	0	4806	15	0.00	14690	39448	37.20
4738	1	\N	\N	\N	15	2025-01-20	2025-01-20 20:00:17	12695	0	24367	20	0.00	12689	37131	34.10
4739	1	\N	\N	\N	15	2025-01-21	2025-01-21 20:00:18	5745	0	17362	11	0.00	13742	37696	15.00
4740	1	\N	\N	\N	15	2025-01-22	2025-01-22 20:00:18	12736	0	25101	11	0.00	12735	39142	32.50
4741	1	\N	\N	\N	15	2025-01-23	2025-01-23 20:00:18	14988	0	6791	9	0.00	15005	38122	39.30
4742	1	\N	\N	\N	15	2025-01-24	2025-01-24 20:00:18	11976	0	18697	11	0.00	11987	39281	30.40
4743	1	\N	\N	\N	15	2025-01-27	2025-01-27 20:00:17	9279	0	25081	10	0.00	9554	38878	23.80
4744	1	\N	\N	\N	15	2025-01-28	2025-01-28 20:00:18	10316	0	22946	15	0.00	10331	38873	26.50
4745	1	\N	\N	\N	15	2025-01-29	2025-01-29 20:00:18	7340	0	5310	17	0.00	7353	39491	18.50
4746	1	\N	\N	\N	15	2025-01-30	2025-01-30 20:00:19	3891	0	22806	0	0.00	14690	39448	9.80
4747	1	\N	\N	\N	15	2025-01-31	2025-01-31 20:00:17	10916	0	25842	0	0.00	10913	39916	27.30
4748	1	\N	\N	\N	16	2025-01-01	2025-01-01 20:00:18	7940	0	26036	9	0.00	9965	38842	20.40
4749	1	\N	\N	\N	16	2025-01-02	2025-01-02 20:00:17	26333	0	10958	11	0.00	11938	39520	66.60
4750	1	\N	\N	\N	16	2025-01-03	2025-01-03 20:00:18	8984	29	24853	6	0.00	8996	38589	23.20
4751	1	\N	\N	\N	16	2025-01-06	2025-01-06 20:00:18	26314	44	12170	5	0.00	19317	37667	69.80
4752	1	\N	\N	\N	16	2025-01-07	2025-01-07 20:00:18	11184	0	19424	6	0.00	11195	36156	31.00
4753	1	\N	\N	\N	16	2025-01-08	2025-01-08 20:00:19	9376	0	26194	23	0.00	9388	38366	24.40
4754	1	\N	\N	\N	16	2025-01-09	2025-01-09 20:00:17	25316	0	7842	11	0.00	10913	39916	64.40
4755	1	\N	\N	\N	16	2025-01-10	2025-01-10 20:00:18	15136	0	23881	5	0.00	15545	43288	35.00
4756	1	\N	\N	\N	16	2025-01-13	2025-01-13 20:00:18	10150	0	25832	23	0.00	10147	37188	27.30
4757	1	\N	\N	\N	16	2025-01-14	2025-01-14 20:00:17	15872	0	21013	10	0.00	15873	39325	40.30
4758	1	\N	\N	\N	16	2025-01-15	2025-01-15 20:00:18	20896	0	14438	0	0.00	20900	40854	51.10
4759	1	\N	\N	\N	16	2025-01-16	2025-01-16 20:00:19	26038	0	14357	9	0.00	19359	38469	67.60
4760	1	\N	\N	\N	16	2025-01-17	2025-01-17 20:00:19	14691	0	22806	0	0.00	14690	39448	37.20
4761	1	\N	\N	\N	16	2025-01-20	2025-01-20 20:00:17	12695	0	24367	5	0.00	12689	37131	34.20
4762	1	\N	\N	\N	16	2025-01-21	2025-01-21 20:00:18	12945	0	17362	11	0.00	13742	37696	34.30
4763	1	\N	\N	\N	16	2025-01-22	2025-01-22 20:00:18	12736	0	14301	8	0.00	12735	39142	32.50
4764	1	\N	\N	\N	16	2025-01-23	2025-01-23 20:00:18	18588	0	13991	9	0.00	15005	38122	48.70
4765	1	\N	\N	\N	16	2025-01-24	2025-01-24 20:00:18	11976	0	18697	15	0.00	11987	39281	30.40
4766	1	\N	\N	\N	16	2025-01-27	2025-01-27 20:00:17	9279	0	25081	0	0.00	9554	38878	23.80
4767	1	\N	\N	\N	16	2025-01-28	2025-01-28 20:00:18	10316	0	22946	8	0.00	10331	38873	26.50
4768	1	\N	\N	\N	16	2025-01-29	2025-01-29 20:00:18	28940	0	5310	4	0.00	7353	39491	73.20
4769	1	\N	\N	\N	16	2025-01-30	2025-01-30 20:00:19	14691	0	4806	5	0.00	14690	39448	37.20
4770	1	\N	\N	\N	16	2025-01-31	2025-01-31 20:00:17	10916	0	25842	11	0.00	10913	39916	27.30
4771	1	\N	\N	\N	17	2025-01-01	2025-01-01 20:00:18	7940	0	26036	30	0.00	9965	38842	20.40
4772	1	\N	\N	\N	17	2025-01-02	2025-01-02 20:00:17	11933	0	10958	25	0.00	11938	39520	30.20
4773	1	\N	\N	\N	17	2025-01-03	2025-01-03 20:00:18	12584	29	17653	8	0.00	8996	38589	32.60
4774	1	\N	\N	\N	17	2025-01-06	2025-01-06 20:00:18	11914	44	8570	8	0.00	19317	37667	31.60
4775	1	\N	\N	\N	17	2025-01-07	2025-01-07 20:00:18	11184	0	19424	23	0.00	11195	36156	31.00
4776	1	\N	\N	\N	17	2025-01-08	2025-01-08 20:00:19	9376	0	26194	12	0.00	9388	38366	24.40
4777	1	\N	\N	\N	17	2025-01-09	2025-01-09 20:00:17	10916	0	15042	13	0.00	10913	39916	27.30
4778	1	\N	\N	\N	17	2025-01-10	2025-01-10 20:00:18	4336	0	23881	18	0.00	15545	43288	10.00
4779	1	\N	\N	\N	17	2025-01-13	2025-01-13 20:00:18	10150	0	25832	11	0.00	10147	37188	27.20
4780	1	\N	\N	\N	17	2025-01-14	2025-01-14 20:00:17	15872	0	21013	14	0.00	15873	39325	40.30
4781	1	\N	\N	\N	17	2025-01-15	2025-01-15 20:00:18	20896	0	14438	2	0.00	20900	40854	51.10
4782	1	\N	\N	\N	17	2025-01-16	2025-01-16 20:00:19	8038	0	10757	2	0.00	19359	38469	20.80
4783	1	\N	\N	\N	17	2025-01-17	2025-01-17 20:00:19	14691	0	22806	9	0.00	14690	39448	37.20
4784	1	\N	\N	\N	17	2025-01-20	2025-01-20 20:00:17	12695	0	9967	0	0.00	12689	37131	34.20
4785	1	\N	\N	\N	17	2025-01-21	2025-01-21 20:00:18	12945	0	17362	0	0.00	13742	37696	34.30
4786	1	\N	\N	\N	17	2025-01-22	2025-01-22 20:00:18	12736	0	25101	6	0.00	12735	39142	32.50
4787	1	\N	\N	\N	17	2025-01-23	2025-01-23 20:00:18	7788	0	21191	18	0.00	15005	38122	20.40
4788	1	\N	\N	\N	17	2025-01-24	2025-01-24 20:00:18	11976	0	18697	19	0.00	11987	39281	30.40
4789	1	\N	\N	\N	17	2025-01-27	2025-01-27 20:00:17	9279	0	25081	17	0.00	9554	38878	23.80
4790	1	\N	\N	\N	17	2025-01-28	2025-01-28 20:00:18	10316	0	22946	12	0.00	10331	38873	26.50
4791	1	\N	\N	\N	17	2025-01-29	2025-01-29 20:00:18	10940	0	8910	5	0.00	7353	39491	27.70
4792	1	\N	\N	\N	17	2025-01-30	2025-01-30 20:00:19	14691	0	22806	9	0.00	14690	39448	372.00
4793	1	\N	\N	\N	17	2025-01-31	2025-01-31 20:00:17	10916	0	25842	21	0.00	10913	39916	27.30
4794	1	\N	\N	\N	18	2025-01-01	2025-01-01 20:00:18	7940	0	15236	21	0.00	9965	38842	20.40
4795	1	\N	\N	\N	18	2025-01-02	2025-01-02 20:00:17	29933	0	3758	12	0.00	11938	39520	75.70
4796	1	\N	\N	\N	18	2025-01-03	2025-01-03 20:00:18	34184	29	3253	12	0.00	8996	38589	88.50
4797	1	\N	\N	\N	18	2025-01-06	2025-01-06 20:00:18	19114	44	15770	23	0.00	19317	37667	50.70
4798	1	\N	\N	\N	18	2025-01-07	2025-01-07 20:00:18	11184	0	19424	18	0.00	11195	36156	30.90
4799	1	\N	\N	\N	18	2025-01-08	2025-01-08 20:00:19	20176	0	8194	4	0.00	9388	38366	52.50
4800	1	\N	\N	\N	18	2025-01-09	2025-01-09 20:00:17	10916	0	25842	0	0.00	10913	39916	27.30
4801	1	\N	\N	\N	18	2025-01-10	2025-01-10 20:00:18	18736	0	9481	0	0.00	15545	43288	43.20
4802	1	\N	\N	\N	18	2025-01-13	2025-01-13 20:00:18	10150	0	25832	8	0.00	10147	37188	27.20
4803	1	\N	\N	\N	18	2025-01-14	2025-01-14 20:00:17	15872	0	21013	0	0.00	15873	39325	40.30
4804	1	\N	\N	\N	18	2025-01-15	2025-01-15 20:00:18	6496	0	3638	9	0.00	20900	40854	15.90
4805	1	\N	\N	\N	18	2025-01-16	2025-01-16 20:00:19	4438	0	7157	9	0.00	19359	38469	11.50
4806	1	\N	\N	\N	18	2025-01-17	2025-01-17 20:00:19	14691	0	22806	15	0.00	14690	39448	37.20
4807	1	\N	\N	\N	18	2025-01-20	2025-01-20 20:00:17	12695	0	24367	16	0.00	12689	37131	34.10
4808	1	\N	\N	\N	18	2025-01-21	2025-01-21 20:00:18	12945	0	17362	11	0.00	13742	37696	34.30
4809	1	\N	\N	\N	18	2025-01-22	2025-01-22 20:00:18	12736	0	25101	16	0.00	12735	39142	32.50
4810	1	\N	\N	\N	18	2025-01-23	2025-01-23 20:00:18	25788	0	6791	9	0.00	15005	38122	67.60
4811	1	\N	\N	\N	18	2025-01-24	2025-01-24 20:00:18	11976	0	18697	15	0.00	11987	39281	30.40
4812	1	\N	\N	\N	18	2025-01-27	2025-01-27 20:00:17	9279	0	25081	10	0.00	9554	38878	23.80
4813	1	\N	\N	\N	18	2025-01-28	2025-01-28 20:00:18	10316	0	22946	17	0.00	10331	38873	26.50
4814	1	\N	\N	\N	18	2025-01-29	2025-01-29 20:00:18	10940	0	5310	1	0.00	7353	39491	27.70
4815	1	\N	\N	\N	18	2025-01-30	2025-01-30 20:00:19	14691	0	22806	0	0.00	14690	39448	37.20
4816	1	\N	\N	\N	18	2025-01-31	2025-01-31 20:00:17	10916	0	25842	23	0.00	10913	39916	27.30
4817	2	\N	\N	\N	19	2025-01-01	2025-01-01 20:00:18	11540	0	26036	6	0.00	9965	38842	29.70
4818	2	\N	\N	\N	19	2025-01-02	2025-01-02 20:00:17	22733	0	10958	2	0.00	11938	39520	57.50
4819	2	\N	\N	\N	19	2025-01-03	2025-01-03 20:00:18	16184	29	24853	11	0.00	8996	38589	41.90
4820	2	\N	\N	\N	19	2025-01-06	2025-01-06 20:00:18	4714	44	15770	11	0.00	19317	37667	12.50
4821	2	\N	\N	\N	19	2025-01-07	2025-01-07 20:00:18	11184	0	19424	12	0.00	11195	36156	30.90
4822	2	\N	\N	\N	19	2025-01-08	2025-01-08 20:00:19	9376	0	18994	4	0.00	9388	38366	24.40
4823	2	\N	\N	\N	19	2025-01-09	2025-01-09 20:00:17	10916	0	25842	4	0.00	10913	39916	27.30
4824	2	\N	\N	\N	19	2025-01-10	2025-01-10 20:00:18	15136	0	23881	4	0.00	15545	43288	34.90
4825	2	\N	\N	\N	19	2025-01-13	2025-01-13 20:00:18	10150	0	4232	12	0.00	10147	37188	27.20
4826	2	\N	\N	\N	19	2025-01-14	2025-01-14 20:00:17	15872	0	21013	10	0.00	15873	39325	40.30
4827	2	\N	\N	\N	19	2025-01-15	2025-01-15 20:00:18	20896	0	14438	18	0.00	20900	40854	51.10
4828	2	\N	\N	\N	19	2025-01-16	2025-01-16 20:00:19	18838	0	17957	21	0.00	19359	38469	48.90
4829	2	\N	\N	\N	19	2025-01-17	2025-01-17 20:00:19	11091	0	4806	21	0.00	14690	39448	28.10
4830	2	\N	\N	\N	19	2025-01-20	2025-01-20 20:00:17	12695	0	24367	21	0.00	12689	37131	34.10
4831	2	\N	\N	\N	19	2025-01-21	2025-01-21 20:00:18	12945	0	17362	11	0.00	13742	37696	34.30
4832	2	\N	\N	\N	19	2025-01-22	2025-01-22 20:00:18	12736	0	7101	22	0.00	12735	39142	32.50
4833	2	\N	\N	\N	19	2025-01-23	2025-01-23 20:00:18	18588	0	17591	9	0.00	15005	38122	48.70
4834	2	\N	\N	\N	19	2025-01-24	2025-01-24 20:00:18	11976	0	18697	8	0.00	11987	39281	30.40
4835	2	\N	\N	\N	19	2025-01-27	2025-01-27 20:00:17	9279	0	25081	5	0.00	9554	38878	23.80
4836	2	\N	\N	\N	19	2025-01-28	2025-01-28 20:00:18	17516	0	4946	0	0.00	10331	38873	45.00
4837	2	\N	\N	\N	19	2025-01-29	2025-01-29 20:00:18	10940	0	8910	9	0.00	7353	39491	27.70
4838	2	\N	\N	\N	19	2025-01-30	2025-01-30 20:00:19	14691	0	22806	5	0.00	14690	39448	37.20
4839	2	\N	\N	\N	19	2025-01-31	2025-01-31 20:00:17	10916	0	25842	1	0.00	10913	39916	27.30
\.


--
-- TOC entry 3382 (class 0 OID 12231668)
-- Dependencies: 235
-- Data for Name: TRN_OPERATION_RESULTS; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."TRN_OPERATION_RESULTS" ("ID", "FACTORY_CD", "SHIFT_ID", "LINE_ID", "PROCESS_ID", "MACHINE_NO", "ACHIEVEMENT_REGISTRATION_DATE", "ACHIEVEMENT_REGISTRATION_TIME", "SLIP_NO", "CUSTOMER_CD", "CUSTOMER_NAME", "DUE_DATE", "PRODUCT_NAME_1", "PRODUCT_NAME_2", "VALUE", "OPERATOR_1", "OPERATOR_2", "OPERATOR_3", "PROCESSING_START_TIME", "PROCESSING_END_TIME", "PROCESSING_TIME", "PROGRESS_RATE", "STANDARD_TIME", "PROCESSING_STOP_TIME", "LOSS_STOP_TIME", "MEASUREMENT_INSPECTION", "CHANGEOVER", "CAD", "EQUIPMENT_FAILURE", "CLEANING", "REST_TIME") FROM stdin;
9257	2	\N	\N	\N	02	2025-04-10	2025-04-10 17:41:18	2120396	\N	(株)松浦機械製作所	2025-04-10	* M4-32650　022-7114A	MCﾅｲﾛﾝ	2				2025-04-10 17:18:01	2025-04-10 17:40:36	1346	100	7200	0	1031	0	0	0	0	0	\N
9296	2	\N	\N	\N	02	2025-04-16	2025-04-16 10:36:23		\N		\N			0				2025-04-16 10:22:53	2025-04-16 10:35:35	756	100	7200	0	206	0	0	0	0	0	\N
9297	2	\N	\N	\N	02	2025-04-16	2025-04-16 12:03:01	2121610	\N	(株)前川製作所　資材課	2025-04-16	６５３−４５１１５−Ａ０ カッパレール	UHMW-PE	1				2025-04-16 10:37:57	2025-04-16 12:02:13	5028	100	7200	0	3650	0	0	0	0	0	\N
9298	2	\N	\N	\N	02	2025-04-16	2025-04-16 13:05:57		\N		\N			0				2025-04-16 10:37:57	2025-04-16 13:05:09	3752	100	0	0	3752	0	0	0	0	0	\N
9299	2	\N	\N	\N	02	2025-04-16	2025-04-16 14:22:34		\N		\N			0				2025-04-16 14:21:48	2025-04-16 14:21:47	4563	100	7200	0	2296	0	0	0	0	0	\N
9300	2	\N	\N	\N	02	2025-04-16	2025-04-16 15:50:02		\N		\N			0				2025-04-16 15:31:53	2025-04-16 15:49:13	1033	100	7200	0	636	0	0	0	0	0	\N
9301	2	\N	\N	\N	02	2025-04-16	2025-04-16 16:06:04	2121606	\N	(株)石野製作所	2025-04-17	【KYH02-F0720G】ｼﾝｸ内側ｶ	POM※対称　	3				2025-04-16 15:50:30	2025-04-16 16:05:16	880	100	7200	0	54	0	0	0	0	0	\N
9302	2	\N	\N	\N	02	2025-04-16	2025-04-16 16:22:01	2120474	\N	高松機械工業株式会社	2025-04-17	Ｌ４５３６２００　ツバガイドＦ	ニューライト	1				2025-04-16 16:11:18	2025-04-16 16:21:12	589	50	7200	0	577	0	0	0	0	0	\N
9303	2	\N	\N	\N	02	2025-04-16	2025-04-16 16:23:47		\N		\N			0				2025-04-16 16:11:18	2025-04-16 16:22:59	103	75	0	0	0	0	0	0	0	0	\N
9304	2	\N	\N	\N	02	2025-04-16	2025-04-16 16:42:51		\N		\N			0				2025-04-16 16:23:05	2025-04-16 16:42:03	1130	50	7200	0	1048	0	0	0	0	0	\N
9305	2	\N	\N	\N	02	2025-04-16	2025-04-16 16:42:54		\N		\N			0				2025-04-16 16:23:05	2025-04-16 16:42:05	1132	0	7200	0	1050	0	0	0	0	0	\N
9306	2	\N	\N	\N	02	2025-04-16	2025-04-16 16:42:57		\N		\N			0				2025-04-16 16:23:05	2025-04-16 16:42:08	3	0	0	0	0	0	0	0	0	0	\N
9274	2	\N	\N	\N	02	2025-04-14	2025-04-14 13:15:13		\N		\N			0				2025-04-14 11:21:57	2025-04-14 13:14:28	6714	50	7200	0	5266	0	0	0	0	0	\N
9275	2	\N	\N	\N	02	2025-04-14	2025-04-14 14:09:19		\N		\N			0	倉田　和旗\n			2025-04-14 13:14:29	2025-04-14 14:08:33	3224	50	0	22	1347	1	0	0	0	0	\N
9276	2	\N	\N	\N	02	2025-04-14	2025-04-14 15:39:29		\N		\N			0	倉田　和旗\n			2025-04-14 14:08:54	2025-04-14 15:38:42	5358	50	7200	0	3289	1	0	0	0	0	\N
9277	2	\N	\N	\N	02	2025-04-14	2025-04-14 16:14:46		\N		\N			0				2025-04-14 14:08:54	2025-04-14 16:14:00	2102	100	0	0	1029	1	0	0	0	0	\N
9278	2	\N	\N	\N	02	2025-04-14	2025-04-14 16:18:16		\N		\N			0				2025-04-14 16:14:02	2025-04-14 16:17:30	205	100	0	0	44	1	0	0	0	0	\N
9279	2	\N	\N	\N	02	2025-04-14	2025-04-14 16:51:15	2113568	\N	株式会社　ＢＢＳ金明	2025-04-15	ＣＰ１４４Ｌ２０１　上枠	POM白	1				2025-04-14 16:18:37	2025-04-14 16:50:29	1899	50	7200	159	348	2	0	0	0	0	\N
9280	2	\N	\N	\N	02	2025-04-14	2025-04-14 17:08:56	2113557	\N	株式会社　ＢＢＳ金明	2025-04-15	ＣＰ１３０Ｌ２２１−２　上枠	POM白	1				2025-04-14 16:50:37	2025-04-14 17:08:10	1045	50	7200	0	373	2	0	0	0	0	\N
9281	2	\N	\N	\N	02	2025-04-14	2025-04-14 17:27:26	2113557	\N	株式会社　ＢＢＳ金明	2025-04-15	ＣＰ１３０Ｌ２２０−２　上枠	ＰＯＭ白　　	1				2025-04-14 17:10:32	2025-04-14 17:26:40	960	50	7200	179	108	3	0	0	0	0	\N
9282	2	\N	\N	\N	02	2025-04-14	2025-04-14 18:05:13	2113568	\N	株式会社　ＢＢＳ金明	2025-04-15	ＣＰ１４４Ｌ２０１　上枠	POM白	1				2025-04-14 17:39:20	2025-04-14 18:04:26	1496	50	7200	0	411	3	0	0	0	0	\N
9283	2	\N	\N	\N	02	2025-04-14	2025-04-14 18:44:53	2113558	\N	株式会社　ＢＢＳ金明	2025-04-15	ＣＰ２０９Ｌ２０３ 下枠	POM白	1				2025-04-14 18:04:39	2025-04-14 18:44:07	2353	50	7200	0	601	3	0	0	0	0	\N
9284	2	\N	\N	\N	02	2025-04-15	2025-04-15 09:32:47		\N		\N			0				2025-04-15 00:00:00	2025-04-15 09:32:01	0	100	0	0	0	0	0	0	0	0	\N
9290	2	\N	\N	\N	02	2025-04-15	2025-04-15 14:53:27	2119490	\N	(株)岩黒製作所	2025-04-15	３Ｓ−８５７−１３８６４　シュートフタ　	ＰＣ　　5X161X205	2				2025-04-15 13:49:34	2025-04-15 14:52:40	3765	100	7200	0	1775	0	0	0	0	0	\N
9291	2	\N	\N	\N	02	2025-04-15	2025-04-15 15:06:24		\N		\N			0				2025-04-15 13:49:34	2025-04-15 15:05:37	769	100	0	0	749	0	0	0	0	0	\N
9292	2	\N	\N	\N	02	2025-04-15	2025-04-15 16:43:55	2120493	\N	サン・プラント工業株式会社	2025-04-16	ＰＲ２３８８６０１００３ ネットガイド板	UHMW	2				2025-04-15 15:05:51	2025-04-15 16:43:09	5806	100	7200	0	4987	0	0	0	0	0	\N
9293	2	\N	\N	\N	02	2025-04-15	2025-04-15 16:44:00		\N		\N			0				2025-04-15 15:05:51	2025-04-15 16:43:13	1	0	0	0	0	0	0	0	0	0	\N
9294	2	\N	\N	\N	02	2025-04-15	2025-04-15 16:44:06		\N		\N			0				2025-04-15 15:05:51	2025-04-15 16:43:20	4	0	0	3	0	1	0	0	0	0	\N
9295	2	\N	\N	\N	02	2025-04-15	2025-04-15 19:07:13		\N		\N			0				2025-04-15 16:43:22	2025-04-15 19:06:26	8538	100	7200	0	5524	1	0	0	0	0	\N
9239	2	\N	\N	\N	02	2025-04-10	2025-04-10 09:38:34		\N		\N			0				2025-04-10 09:12:03	2025-04-10 09:37:52	1539	50	7200	0	297	0	0	0	0	0	\N
9240	2	\N	\N	\N	02	2025-04-10	2025-04-10 09:47:05		\N		\N			0				2025-04-10 09:37:54	2025-04-10 09:46:23	504	100	7200	0	246	0	0	0	0	0	\N
9241	2	\N	\N	\N	02	2025-04-10	2025-04-10 10:00:12		\N		\N			0				2025-04-10 09:48:12	2025-04-10 09:59:30	673	100	7200	0	398	0	0	0	0	0	\N
9242	2	\N	\N	\N	02	2025-04-10	2025-04-10 10:00:17		\N		\N			0				2025-04-10 09:48:12	2025-04-10 09:59:35	2	0	0	0	0	0	0	0	0	0	\N
9243	2	\N	\N	\N	02	2025-04-10	2025-04-10 10:00:22		\N		\N			0				2025-04-10 09:48:12	2025-04-10 09:59:40	1	0	0	0	0	0	0	0	0	0	\N
9244	2	\N	\N	\N	02	2025-04-10	2025-04-10 10:14:47		\N		\N			0				2025-04-10 09:59:42	2025-04-10 10:14:05	856	100	7200	0	818	0	0	0	0	0	\N
9245	2	\N	\N	\N	02	2025-04-10	2025-04-10 10:34:19	2118434	\N	高松機械工業株式会社	2025-04-10	Ｌ３５４４３７０　シート７	ジュラコン	5				2025-04-10 10:27:24	2025-04-10 10:33:37	369	100	7200	0	116	0	0	0	0	0	\N
9246	2	\N	\N	\N	02	2025-04-10	2025-04-10 10:40:49	2118434	\N	高松機械工業株式会社	2025-04-10	Ｌ３５４４３７０　シート７	ジュラコン	2				2025-04-10 10:33:45	2025-04-10 10:40:07	378	100	7200	0	135	0	0	0	0	0	\N
9247	2	\N	\N	\N	02	2025-04-10	2025-04-10 11:13:21		\N		\N			0				2025-04-10 10:41:46	2025-04-10 11:12:38	1841	75	7200	0	252	0	0	0	0	0	\N
9248	2	\N	\N	\N	02	2025-04-10	2025-04-10 12:01:39		\N		\N			0				2025-04-10 11:18:56	2025-04-10 12:00:56	2505	50	7200	0	776	0	0	0	0	0	\N
9249	2	\N	\N	\N	02	2025-04-10	2025-04-10 13:01:12		\N		\N			0				2025-04-10 12:01:00	2025-04-10 13:00:29	3549	75	7200	0	3181	0	0	0	0	0	\N
9250	2	\N	\N	\N	02	2025-04-10	2025-04-10 14:07:50		\N		\N			0				2025-04-10 13:05:10	2025-04-10 14:07:07	4	100	0	0	0	0	0	0	0	0	\N
9251	2	\N	\N	\N	02	2025-04-10	2025-04-10 14:07:53		\N		\N			0				2025-04-10 13:05:10	2025-04-10 14:07:10	7	0	0	0	0	0	0	0	0	0	\N
9252	2	\N	\N	\N	02	2025-04-10	2025-04-10 15:07:31	2119046	\N	(株)馬場鐵工所　川北工場	2025-04-12	Ｕ３０４６２００　ガイド	MC901	1				2025-04-10 14:07:18	2025-04-10 15:06:48	3550	50	7200	0	1495	0	0	0	0	0	\N
9253	2	\N	\N	\N	02	2025-04-10	2025-04-10 15:09:19		\N		\N			0				2025-04-10 14:07:18	2025-04-10 15:08:37	104	100	0	0	0	0	0	0	0	0	\N
9254	2	\N	\N	\N	02	2025-04-10	2025-04-10 15:09:23		\N		\N			0				2025-04-10 14:07:18	2025-04-10 15:08:40	107	0	0	0	0	0	0	0	0	0	\N
9255	2	\N	\N	\N	02	2025-04-10	2025-04-10 16:11:42	2119046	\N	(株)馬場鐵工所　川北工場	2025-04-10	Ｕ３０５２５１０　ガイド	MC901	1				2025-04-10 15:09:02	2025-04-10 16:11:00	3695	50	7200	0	1548	0	0	0	0	0	\N
9256	2	\N	\N	\N	02	2025-04-10	2025-04-10 16:44:37		\N		\N			0				2025-04-10 16:28:11	2025-04-10 16:43:54	936	100	7200	0	636	0	0	0	0	0	\N
9258	2	\N	\N	\N	02	2025-04-10	2025-04-10 08:42:39		\N		\N			0				2025-04-10 08:35:30	2025-04-10 08:41:55	382	100	7200	0	299	0	0	0	0	0	\N
9221	2	\N	\N	\N	02	2025-04-01	2025-04-01 09:38:34		\N		\N			0				2025-04-01 09:12:03	2025-04-01 09:37:52	1539	50	7200	0	297	0	0	0	0	0	\N
9222	2	\N	\N	\N	02	2025-04-01	2025-04-01 09:47:05		\N		\N			0				2025-04-01 09:37:54	2025-04-01 09:46:23	504	100	7200	0	246	0	0	0	0	0	\N
9223	2	\N	\N	\N	02	2025-04-01	2025-04-01 10:00:12		\N		\N			0				2025-04-01 09:48:12	2025-04-01 09:59:30	673	100	7200	0	398	0	0	0	0	0	\N
9224	2	\N	\N	\N	02	2025-04-01	2025-04-01 10:00:17		\N		\N			0				2025-04-01 09:48:12	2025-04-01 09:59:35	2	0	0	0	0	0	0	0	0	0	\N
9225	2	\N	\N	\N	02	2025-04-01	2025-04-01 10:00:22		\N		\N			0				2025-04-01 09:48:12	2025-04-01 09:59:40	1	0	0	0	0	0	0	0	0	0	\N
9226	2	\N	\N	\N	02	2025-04-01	2025-04-01 10:14:47		\N		\N			0				2025-04-01 09:59:42	2025-04-01 10:14:05	856	100	7200	0	818	0	0	0	0	0	\N
9227	2	\N	\N	\N	02	2025-04-01	2025-04-01 10:34:19	2118434	\N	高松機械工業株式会社	2025-04-10	Ｌ３５４４３７０　シート７	ジュラコン	5				2025-04-01 10:27:24	2025-04-01 10:33:37	369	100	7200	0	116	0	0	0	0	0	\N
9228	2	\N	\N	\N	02	2025-04-01	2025-04-01 10:40:49	2118434	\N	高松機械工業株式会社	2025-04-10	Ｌ３５４４３７０　シート７	ジュラコン	2				2025-04-01 10:33:45	2025-04-01 10:40:07	378	100	7200	0	135	0	0	0	0	0	\N
9229	2	\N	\N	\N	02	2025-04-01	2025-04-01 11:13:21		\N		\N			0				2025-04-01 10:41:46	2025-04-01 11:12:38	1841	75	7200	0	252	0	0	0	0	0	\N
9230	2	\N	\N	\N	02	2025-04-01	2025-04-01 12:01:39		\N		\N			0				2025-04-01 11:18:56	2025-04-01 12:00:56	2505	50	7200	0	776	0	0	0	0	0	\N
9231	2	\N	\N	\N	02	2025-04-01	2025-04-01 13:01:12		\N		\N			0				2025-04-01 12:01:00	2025-04-01 13:00:29	3549	75	7200	0	3181	0	0	0	0	0	\N
9232	2	\N	\N	\N	02	2025-04-01	2025-04-01 14:07:50		\N		\N			0				2025-04-01 13:05:10	2025-04-01 14:07:07	4	100	0	0	0	0	0	0	0	0	\N
9233	2	\N	\N	\N	02	2025-04-01	2025-04-01 14:07:53		\N		\N			0				2025-04-01 13:05:10	2025-04-01 14:07:10	7	0	0	0	0	0	0	0	0	0	\N
9234	2	\N	\N	\N	02	2025-04-01	2025-04-01 15:07:31	2119046	\N	(株)馬場鐵工所　川北工場	2025-04-12	Ｕ３０４６２００　ガイド	MC901	1				2025-04-01 14:07:18	2025-04-01 15:06:48	3550	50	7200	0	1495	0	0	0	0	0	\N
9235	2	\N	\N	\N	02	2025-04-01	2025-04-01 15:09:19		\N		\N			0				2025-04-01 14:07:18	2025-04-01 15:08:37	104	100	0	0	0	0	0	0	0	0	\N
9236	2	\N	\N	\N	02	2025-04-01	2025-04-01 15:09:23		\N		\N			0				2025-04-01 14:07:18	2025-04-01 15:08:40	107	0	0	0	0	0	0	0	0	0	\N
9237	2	\N	\N	\N	02	2025-04-01	2025-04-01 16:44:37		\N		\N			0				2025-04-01 16:28:11	2025-04-01 16:43:54	936	100	7200	0	636	0	0	0	0	0	\N
9238	2	\N	\N	\N	02	2025-04-02	2025-04-02 08:42:39		\N		\N			0				2025-04-02 08:35:30	2025-04-02 08:41:55	382	100	7200	0	299	0	0	0	0	0	\N
9273	2	\N	\N	\N	02	2025-04-14	2025-04-14 10:24:53	2120918	\N	豊田産業(株)　一色工場	2025-04-15	J1314-11010-0Ａ　カッター子	ＭＣナイロン *全数検査	1				2025-04-14 09:02:19	2025-04-14 10:24:08	4881	75	7200	0	2831	0	0	0	0	0	\N
9259	2	\N	\N	\N	02	2025-04-11	2025-04-11 09:19:59	2117547	\N	澁谷工業(株)メカトロ生産本部	2025-04-14	９２６８４２３５２８００　ｽﾄｯﾊﾟﾌﾞ	MC901	3				2025-04-11 08:59:46	2025-04-11 09:19:15	1161	100	7200	0	1117	0	0	0	0	0	\N
9260	2	\N	\N	\N	02	2025-04-11	2025-04-11 09:30:38	2117723	\N	澁谷工業(株)メカトロ生産本部	2025-04-14	９５９２４６４９３０６０　CGｺﾈｸﾀﾌ	POM黒	1				2025-04-11 09:20:53	2025-04-11 09:29:54	536	100	7200	0	351	1	0	0	0	0	\N
9261	2	\N	\N	\N	02	2025-04-11	2025-04-11 10:00:10	2117726	\N	澁谷工業(株)メカトロ生産本部	2025-04-14	９１４１４５６９１０００　ガイド	ＮＬ−Ｗ	1				2025-04-11 09:54:13	2025-04-11 09:59:26	309	100	7200	0	81	1	0	0	0	0	\N
9262	2	\N	\N	\N	02	2025-04-11	2025-04-11 10:46:57		\N		\N			0				2025-04-11 10:31:34	2025-04-11 10:46:13	30	75	0	0	0	1	0	0	0	0	\N
9263	2	\N	\N	\N	02	2025-04-11	2025-04-11 10:48:08		\N		\N			0				2025-04-11 10:31:34	2025-04-11 10:47:24	68	75	0	0	0	1	0	0	0	0	\N
9264	2	\N	\N	\N	02	2025-04-11	2025-04-11 10:48:13		\N		\N			0				2025-04-11 10:31:34	2025-04-11 10:47:30	2	0	0	0	0	1	0	0	0	0	\N
9265	2	\N	\N	\N	02	2025-04-11	2025-04-11 11:50:00	2114747	\N	@Oì»ì@Þ	2025-04-14	RRO|QQOOW|	UHMW-PE (F)	2				2025-04-11 11:32:13	2025-04-11 11:49:16	1014	100	7200	0	267	1	0	0	0	0	\N
9266	2	\N	\N	\N	02	2025-04-11	2025-04-11 13:14:32	2120186	\N	宮岸株式会社（富山支店）	2025-04-13	251N-23104222　K-6101	ニューライト	1				2025-04-11 11:49:28	2025-04-11 13:13:48	5033	100	7200	0	4005	1	0	0	0	0	\N
9267	2	\N	\N	\N	02	2025-04-11	2025-04-11 13:52:21		\N		\N			0				2025-04-11 13:13:54	2025-04-11 13:51:37	2249	100	7200	7	660	2	0	0	0	0	\N
9268	2	\N	\N	\N	02	2025-04-11	2025-04-11 15:58:22		\N		\N			0				2025-04-11 14:04:11	2025-04-11 15:57:38	6771	100	7200	0	4346	2	0	0	0	0	\N
9269	2	\N	\N	\N	02	2025-04-11	2025-04-11 17:23:05		\N		\N			0				2025-04-11 16:52:48	2025-04-11 17:22:21	1761	50	7200	0	783	2	0	0	0	0	\N
9270	2	\N	\N	\N	02	2025-04-11	2025-04-11 18:06:05		\N		\N			0	倉田　和旗\n			2025-04-11 17:22:22	2025-04-11 18:05:20	2563	100	0	0	1440	2	0	0	0	0	\N
9271	2	\N	\N	\N	02	2025-04-11	2025-04-11 18:49:32		\N		\N			0				2025-04-11 00:00:00	2025-04-11 18:48:48	0	75	0	0	0	0	0	0	0	0	\N
9272	2	\N	\N	\N	02	2025-04-11	2025-04-11 18:49:37		\N		\N			0				2025-04-11 00:00:00	2025-04-11 18:48:53	0	75	0	0	0	0	0	0	0	0	\N
9285	2	\N	\N	\N	02	2025-04-15	2025-04-15 10:28:44		\N		\N			0				2025-04-15 09:32:06	2025-04-15 10:27:58	3333	75	7200	0	2328	0	0	0	0	0	\N
9286	2	\N	\N	\N	02	2025-04-15	2025-04-15 10:28:57		\N		\N			0				2025-04-15 09:32:06	2025-04-15 10:28:11	10	0	0	0	0	0	0	0	0	0	\N
9287	2	\N	\N	\N	02	2025-04-15	2025-04-15 10:46:05	2120978	\N	株式会社ジェイテクト	2025-04-15	ＴＤ-６ＦＪＥ００６９-Ａ ガイド	ＰＥＥＫ	3				2025-04-15 10:30:10	2025-04-15 10:45:19	902	100	7200	0	69	0	0	0	0	0	\N
9288	2	\N	\N	\N	02	2025-04-15	2025-04-15 11:09:02	2120978	\N	株式会社ジェイテクト	2025-04-15	ＴＤ-６ＦＪＥ００７０-Ａ ガイド	ＰＥＥＫ	1				2025-04-15 10:47:54	2025-04-15 11:08:16	1214	100	7200	0	1178	0	0	0	0	0	\N
9289	2	\N	\N	\N	02	2025-04-15	2025-04-15 13:50:06	2120978	\N	株式会社ジェイテクト	2025-04-15	ＴＤ-６Ｅ０Ｅ０７９１-Ａ プレート	ＰＥＥＫ	1				2025-04-15 12:02:13	2025-04-15 13:49:19	6391	100	7200	0	5678	0	0	0	0	0	\N
9327	2	\N	\N	\N	21	2025-04-01	2025-04-01 08:56:19		\N		\N			0				2025-04-01 09:08:02	2025-04-01 09:24:31	982	100	7200	0	763	0	0	0	0	0	\N
9328	2	\N	\N	\N	21	2025-04-01	2025-04-01 09:07:50	2114335	\N	ゼネラルパッカー株式会社	2025-04-20	MY-0-39-100-A1(MY-00	ジュラコン	7				2025-04-01 09:24:46	2025-04-01 09:34:08	557	100	7200	0	330	0	0	0	0	0	\N
9329	2	\N	\N	\N	21	2025-04-01	2025-04-01 09:39:27	2114335	\N	ゼネラルパッカー株式会社	2025-04-20	MY-0-39-100-A1(MY-00	ジュラコン	8				2025-04-01 09:34:09	2025-04-01 09:43:12	538	100	0	0	226	0	0	0	0	0	\N
9330	2	\N	\N	\N	21	2025-04-01	2025-04-01 10:16:48		\N		\N			0				2025-04-01 09:34:09	2025-04-01 09:55:33	735	100	0	0	0	0	0	0	0	0	\N
9331	2	\N	\N	\N	21	2025-04-01	2025-04-01 10:32:45		\N		\N			0				2025-04-01 09:34:09	2025-04-01 09:55:37	1	100	0	0	0	0	0	0	0	0	\N
9332	2	\N	\N	\N	21	2025-04-01	2025-04-01 10:45:49	2114335	\N	ゼネラルパッカー株式会社	2025-04-20	MY-0-39-100-A1(MY-00	ジュラコン	1				2025-04-01 09:55:42	2025-04-01 10:23:40	1667	100	7200	0	1126	0	0	0	0	0	\N
9333	2	\N	\N	\N	21	2025-04-01	2025-04-01 10:46:11		\N		\N			0				2025-04-01 00:00:00	2025-04-01 10:52:45	0	0	0	0	0	0	0	0	0	0	\N
9334	2	\N	\N	\N	21	2025-04-01	2025-04-01 11:03:24	2116681	\N	澁谷工業(株)メカトロ生産本部	2025-04-20	９２８３６９１５Ｇ０５０　ｵｻｴﾌﾟﾚｰ	TI5013	2				2025-04-01 10:52:57	2025-04-01 11:14:16	1270	100	7200	0	666	0	0	0	0	0	\N
9335	2	\N	\N	\N	21	2025-04-01	2025-04-01 11:09:15	2117582	\N	株式会社根上シブヤ	2025-04-20	６３３９２Ｈ３０１０１０　ガイド	ＮＬ-Ｗ	16				2025-04-01 11:15:53	2025-04-01 14:26:33	0	100	7200	0	11382	0	0	0	0	0	\N
9336	2	\N	\N	\N	21	2025-04-01	2025-04-01 11:20:55	2117582	\N	株式会社根上シブヤ	2025-04-20	６３３９２Ｈ３０１０１０　ガイド	ＮＬ-Ｗ	5				2025-04-01 14:26:35	2025-04-01 14:31:55	317	100	7200	0	221	0	0	0	0	0	\N
9337	2	\N	\N	\N	21	2025-04-01	2025-04-01 11:21:11	2113647	\N	àFJHÆvgÇ	2025-04-20	S|VRTPUQRW	PET§¾	2				2025-04-01 14:32:05	2025-04-01 14:39:30	442	100	7200	0	270	0	0	0	0	0	\N
9338	2	\N	\N	\N	21	2025-04-01	2025-04-01 11:25:35		\N		\N			0				2025-04-01 14:32:05	2025-04-01 14:42:31	177	100	0	0	0	0	0	0	0	0	\N
9339	2	\N	\N	\N	21	2025-04-01	2025-04-01 13:02:44	2115766	\N	シブヤパッケージングシステム(株)	2025-04-20	FSY2040　３−Ｂ７７５４６−０　ｺ	ﾆｭｰﾗｲﾄ　　3-B77546-001	3	倉田　和旗\n			2025-04-01 14:42:37	2025-04-01 18:23:39	13192	100	7200	0	8822	0	0	0	0	0	\N
9340	2	\N	\N	\N	21	2025-04-01	2025-04-01 13:23:16	2115785	\N	シブヤ精機株式会社	2025-04-20	Ｎ４２Ａ１１４９Ｓ０００ ケリダシササエ	MCﾅｲﾛﾝ 	2				2025-04-01 18:33:41	2025-04-01 18:55:38	1307	75	7200	0	753	0	0	0	0	0	\N
9341	2	\N	\N	\N	21	2025-04-01	2025-04-01 13:23:36	2118609	\N	中村留精密工業株式会社	2025-04-20	ＴＴＳＲ０１３６２０１２　コツメ（29）	PA/MC(MC901)	3				2025-04-01 08:31:29	2025-04-01 09:05:13	2010	75	7200	0	624	0	0	0	0	0	\N
9307	2	\N	\N	\N	02	2025-04-16	2025-04-16 17:23:54	2120472	\N	高松機械工業株式会社	2025-04-17	Ｌ３８５７９５０　Ｖ受け	ニューライト	1				2025-04-16 16:43:07	2025-04-16 17:23:05	0	75	7200	0	2384	0	0	0	0	0	\N
9308	2	\N	\N	\N	02	2025-04-16	2025-04-16 18:05:10	2119878	\N	シブヤパッケージングシステム(株)	2025-04-17	FSV2992　4-C44173-1/4	ＰＥ　	1				2025-04-16 17:54:23	2025-04-16 18:04:21	594	100	7200	0	158	0	0	0	0	0	\N
9309	2	\N	\N	\N	02	2025-04-16	2025-04-16 18:10:22		\N		\N			0				2025-04-16 17:54:23	2025-04-16 18:09:33	308	0	0	0	27	0	0	0	0	0	\N
9406	1	\N	\N	\N	01	2025-04-08	2025-04-08 10:33:03	2118942	\N	澁谷工業(株)メカトロ生産本部	2025-04-10	９２８３０７１７Ｋ２４０　ホルダ	MEP-POM(ｸﾛ)	1				2025-04-08 10:27:02	2025-04-08 10:32:23	318	50	7200	0	75	0	0	0	0	0	\N
\.


--
-- TOC entry 3398 (class 0 OID 0)
-- Dependencies: 228
-- Name: MST_FACTORY_ID_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public."MST_FACTORY_ID_seq"', 2, true);


--
-- TOC entry 3399 (class 0 OID 0)
-- Dependencies: 230
-- Name: MST_MACHINE_ID_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public."MST_MACHINE_ID_seq"', 21, true);


--
-- TOC entry 3400 (class 0 OID 0)
-- Dependencies: 238
-- Name: TRN_IMPORT_HISTORY_DETAIL_ID_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public."TRN_IMPORT_HISTORY_DETAIL_ID_seq"', 14613, true);


--
-- TOC entry 3401 (class 0 OID 0)
-- Dependencies: 236
-- Name: TRN_IMPORT_HISTORY_ID_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public."TRN_IMPORT_HISTORY_ID_seq"', 3608, true);


--
-- TOC entry 3402 (class 0 OID 0)
-- Dependencies: 232
-- Name: TRN_OPERATION_OEE_ID_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public."TRN_OPERATION_OEE_ID_seq"', 5295, true);


--
-- TOC entry 3403 (class 0 OID 0)
-- Dependencies: 234
-- Name: TRN_OPERATION_RESULTS_ID_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public."TRN_OPERATION_RESULTS_ID_seq"', 11815, true);


--
-- TOC entry 3232 (class 2606 OID 15935320)
-- Name: TRN_IMPORT_HISTORY_DETAIL TRN_IMPORT_HISTORY_DETAIL_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."TRN_IMPORT_HISTORY_DETAIL"
    ADD CONSTRAINT "TRN_IMPORT_HISTORY_DETAIL_pkey" PRIMARY KEY ("ID");


--
-- TOC entry 3230 (class 2606 OID 13898274)
-- Name: TRN_IMPORT_HISTORY importhistory_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."TRN_IMPORT_HISTORY"
    ADD CONSTRAINT importhistory_pkey PRIMARY KEY ("ID");


--
-- TOC entry 3222 (class 2606 OID 12231609)
-- Name: MST_FACTORY mst_factory_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."MST_FACTORY"
    ADD CONSTRAINT mst_factory_pkey PRIMARY KEY ("FACTORY_CD");


--
-- TOC entry 3224 (class 2606 OID 12231625)
-- Name: MST_MACHINE mst_machine_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."MST_MACHINE"
    ADD CONSTRAINT mst_machine_pkey PRIMARY KEY ("HMI_NO", "MACHINE_NO");


--
-- TOC entry 3226 (class 2606 OID 12231666)
-- Name: TRN_OPERATION_OEE trn_operation_oee_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."TRN_OPERATION_OEE"
    ADD CONSTRAINT trn_operation_oee_pkey PRIMARY KEY ("FACTORY_CD", "MACHINE_NO", "ID");


--
-- TOC entry 3228 (class 2606 OID 12231673)
-- Name: TRN_OPERATION_RESULTS trn_operation_results_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."TRN_OPERATION_RESULTS"
    ADD CONSTRAINT trn_operation_results_pkey PRIMARY KEY ("FACTORY_CD", "MACHINE_NO", "ID");


-- Completed on 2025-07-15 15:47:44

--
-- PostgreSQL database dump complete
--

