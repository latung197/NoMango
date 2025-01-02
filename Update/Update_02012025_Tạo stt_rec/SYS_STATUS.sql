Create table SYS_STATUS (
	
	unit_id ud_id,
	Voucher_ID ud_char3,
	status_id ud_status primary key,
	status_name ud_name,
	status ud_status,
	create_id ud_id,
	create_date ud_date,
	create_time ud_time,
	update_id ud_id,
	update_date ud_date,
	update_time ud_time
)