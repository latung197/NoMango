using System.Data;
using System.Collections.Generic;
using ERP_System.Models;

namespace ERP_System.Repositories.Inventory
{
    public class StockRequestRepository : BaseRepository
    {
        public List<StockRequest> GetStockRequests()
        {
            var requests = new List<StockRequest>();
            var query = "SELECT * FROM vw_stock_requests ORDER BY request_date DESC, request_code DESC";

            var dataTable = ExecuteQuery(query);

            foreach (DataRow row in dataTable.Rows)
            {
                requests.Add(new StockRequest
                {
                    RequestID = row.Field<int>("request_id"),
                    RequestCode = row.Field<string>("request_code"),
                    RequestDate = row.Field<DateTime>("request_date"),
                    WarehouseID = row.Field<int>("warehouse_id"),
                    WarehouseName = row.Field<string>("warehouse_name"),
                    Department =  row.Field<string>("department"),
                    Reason =  row.Field<string>("reason"),
                    Description =  row.Field<string>("description"),
                    Status = row.Field<string>("status"),
                    TotalQuantity = row.Field<decimal>("total_quantity"),
                    TotalValue = row.Field<decimal>("total_value"),
                    CreatedBy = row.Field<int>("created_by"),
                    CreatedByName = row.Field<string>("created_by_name"),
                    ApprovedBy =  row.Field<int>("approved_by"),
                    ApprovedByName = row.Field<string>("approved_by_name"),
                    CreatedDate = row.Field<DateTime>("created_date"),
                    ApprovedDate = row.Field<DateTime>("approved_date")
                });
            }

            return requests;
        }

        public StockRequest GetStockRequest(int requestID)
        {
            var request = new StockRequest();
            var query = @"
                SELECT * FROM vw_stock_requests WHERE request_id = @request_id;
                SELECT * FROM vw_stock_request_details WHERE request_id = @request_id ORDER BY sort_order;";

            var parameters = new Dictionary<string, object>
            {
                { "@request_id", requestID }
            };

            var dataSet = new DataSet();
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new Npgsql.NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@request_id", requestID);
                    using (var adapter = new Npgsql.NpgsqlDataAdapter(cmd))
                    {
                        adapter.Fill(dataSet);
                    }
                }
            }

            if (dataSet.Tables[0].Rows.Count > 0)
            {
                var row = dataSet.Tables[0].Rows[0];
                request.RequestID = row.Field<int>("request_id");
                request.RequestCode = row.Field<string>("request_code");
                request.RequestDate = row.Field<DateTime>("request_date");
                request.WarehouseID = row.Field<int>("warehouse_id");
                request.WarehouseName = row.Field<string>("warehouse_name");
                request.Department =  row.Field<string>("department");
                request.Reason =  row.Field<string>("reason");
                request.Description =  row.Field<string>("description");
                request.Status = row.Field<string>("status");
                request.TotalQuantity = row.Field<decimal>("total_quantity");
                request.TotalValue = row.Field<decimal>("total_value");
                request.CreatedBy = row.Field<int>("created_by");
                request.CreatedByName = row.Field<string>("created_by_name");
                request.ApprovedBy = row.Field<int>("approved_by");
                request.ApprovedByName = row.Field<string>("approved_by_name");
                request.CreatedDate = row.Field<DateTime>("created_date");
                request.ApprovedDate = row.Field<DateTime?>("approved_date");

                // Load details
                foreach (DataRow detailRow in dataSet.Tables[1].Rows)
                {
                    request.Details.Add(new StockRequestDetail
                    {
                        DetailID = detailRow.Field<int>("detail_id"),
                        RequestID = detailRow.Field<int>("request_id"),
                        MaterialID = detailRow.Field<int>("material_id"),
                        MaterialCode = detailRow.Field<string>("material_code"),
                        MaterialName = detailRow.Field<string>("material_name"),
                        UnitName = detailRow.Field<string>("unit_name"),
                        Quantity = detailRow.Field<decimal>("quantity"),
                        UnitPrice = detailRow.Field<decimal>("unit_price"),
                        TotalPrice = detailRow.Field<decimal>("total_price"),
                        Note =  detailRow.Field<string>("note"),
                        SortOrder = detailRow.Field<int>("sort_order")
                    });
                }
            }

            return request;
        }

        public string GenerateRequestCode()
        {
            var query = "SELECT generate_code('XK', 'stock_requests', 'request_code')";
            var result = ExecuteScalar(query);
            return result?.ToString() ?? "XK0001";
        }

        public int CreateStockRequest(StockRequest request)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // Insert main request
                        var query = @"
                            INSERT INTO stock_requests (request_code, request_date, warehouse_id, department, 
                                                     reason, description, status, created_by)
                            VALUES (@request_code, @request_date, @warehouse_id, @department,
                                   @reason, @description, @status, @created_by)
                            RETURNING request_id";

                        var parameters = new Dictionary<string, object>
                        {
                            { "@request_code", request.RequestCode },
                            { "@request_date", request.RequestDate },
                            { "@warehouse_id", request.WarehouseID },
                            { "@department", request.Department ?? (object)DBNull.Value },
                            { "@reason", request.Reason ?? (object)DBNull.Value },
                            { "@description", request.Description ?? (object)DBNull.Value },
                            { "@status", request.Status },
                            { "@created_by", request.CreatedBy }
                        };

                        var requestID = ExecuteScalar(query, parameters);

                        // Insert details
                        foreach (var detail in request.Details)
                        {
                            query = @"
                                INSERT INTO stock_request_details (request_id, material_id, quantity, 
                                                                 unit_price, total_price, note, sort_order)
                                VALUES (@request_id, @material_id, @quantity, @unit_price, 
                                       @total_price, @note, @sort_order)";

                            parameters = new Dictionary<string, object>
                            {
                                { "@request_id", requestID },
                                { "@material_id", detail.MaterialID },
                                { "@quantity", detail.Quantity },
                                { "@unit_price", detail.UnitPrice },
                                { "@total_price", detail.TotalPrice },
                                { "@note", detail.Note ?? (object)DBNull.Value },
                                { "@sort_order", detail.SortOrder }
                            };

                            ExecuteNonQuery(query, parameters);
                        }

                        transaction.Commit();
                        return Convert.ToInt32(requestID);
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public bool UpdateStockRequest(StockRequest request)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // Update main request
                        var query = @"
                            UPDATE stock_requests 
                            SET request_date = @request_date, warehouse_id = @warehouse_id,
                                department = @department, reason = @reason, description = @description,
                                status = @status
                            WHERE request_id = @request_id";

                        var parameters = new Dictionary<string, object>
                        {
                            { "@request_id", request.RequestID },
                            { "@request_date", request.RequestDate },
                            { "@warehouse_id", request.WarehouseID },
                            { "@department", request.Department ?? (object)DBNull.Value },
                            { "@reason", request.Reason ?? (object)DBNull.Value },
                            { "@description", request.Description ?? (object)DBNull.Value },
                            { "@status", request.Status }
                        };

                        ExecuteNonQuery(query, parameters);

                        // Delete existing details
                        query = "DELETE FROM stock_request_details WHERE request_id = @request_id";
                        parameters = new Dictionary<string, object> { { "@request_id", request.RequestID } };
                        ExecuteNonQuery(query, parameters);

                        // Insert new details
                        foreach (var detail in request.Details)
                        {
                            query = @"
                                INSERT INTO stock_request_details (request_id, material_id, quantity, 
                                                                 unit_price, total_price, note, sort_order)
                                VALUES (@request_id, @material_id, @quantity, @unit_price, 
                                       @total_price, @note, @sort_order)";

                            parameters = new Dictionary<string, object>
                            {
                                { "@request_id", request.RequestID },
                                { "@material_id", detail.MaterialID },
                                { "@quantity", detail.Quantity },
                                { "@unit_price", detail.UnitPrice },
                                { "@total_price", detail.TotalPrice },
                                { "@note", detail.Note ?? (object)DBNull.Value },
                                { "@sort_order", detail.SortOrder }
                            };

                            ExecuteNonQuery(query, parameters);
                        }

                        transaction.Commit();
                        return true;
                    }
                    catch
                    {
                        transaction.Rollback();
                        return false;
                    }
                }
            }
        }

        public bool ApproveStockRequest(int requestID, int approvedBy)
        {
            var query = @"
                UPDATE stock_requests 
                SET status = 'Approved', approved_by = @approved_by, approved_date = CURRENT_TIMESTAMP
                WHERE request_id = @request_id";

            var parameters = new Dictionary<string, object>
            {
                { "@request_id", requestID },
                { "@approved_by", approvedBy }
            };

            return ExecuteNonQuery(query, parameters) > 0;
        }

        public bool DeleteStockRequest(int requestID)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // Delete details first
                        var query = "DELETE FROM stock_request_details WHERE request_id = @request_id";
                        var parameters = new Dictionary<string, object> { { "@request_id", requestID } };
                        ExecuteNonQuery(query, parameters);

                        // Delete main request
                        query = "DELETE FROM stock_requests WHERE request_id = @request_id";
                        ExecuteNonQuery(query, parameters);

                        transaction.Commit();
                        return true;
                    }
                    catch
                    {
                        transaction.Rollback();
                        return false;
                    }
                }
            }
        }
    }
}