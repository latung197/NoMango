using System.Data;
using System.Collections.Generic;
using ERP_System.Models;

namespace ERP_System.Repositories.Inventory
{
    public class MaterialRepository : BaseRepository
    {
        public List<Material> GetMaterials()
        {
            var materials = new List<Material>();
            var query = "SELECT * FROM vw_materials ORDER BY material_code";

            var dataTable = ExecuteQuery(query);

            foreach (DataRow row in dataTable.Rows)
            {
                materials.Add(new Material
                {
                    MaterialID = row.Field<int>("material_id"),
                    MaterialCode = row.Field<string>("material_code"),
                    MaterialName = row.Field<string>("material_name"),
                    UnitID = row.Field<int>("unit_id"),
                    UnitName = row.Field<string>("unit_name"),
                    MaterialGroupID = row.Field<int>("group_id"),
                    MaterialGroup = row.Field<string>("group_name"),
                    MinStock = row.Field<decimal>("min_stock"),
                    MaxStock = row.Field<decimal>("max_stock"),
                    CurrentStock = row.Field<decimal>("current_stock"),
                    CostPrice = row.Field<decimal>("cost_price"),
                    SellingPrice = row.Field<decimal>("selling_price"),
                    IsActive = row.Field<bool>("is_active"),
                    StockStatus = row.Field<string>("stock_status")
                });
            }

            return materials;
        }

        public Material GetMaterial(int materialID)
        {
            var query = "SELECT * FROM vw_materials WHERE material_id = @material_id";
            var parameters = new Dictionary<string, object>
            {
                { "@material_id", materialID }
            };

            var dataTable = ExecuteQuery(query, parameters);

            if (dataTable.Rows.Count > 0)
            {
                var row = dataTable.Rows[0];
                return new Material
                {
                    MaterialID = row.Field<int>("material_id"),
                    MaterialCode = row.Field<string>("material_code"),
                    MaterialName = row.Field<string>("material_name"),
                    UnitID = row.Field<int>("unit_id"),
                    UnitName = row.Field<string>("unit_name"),
                    MaterialGroupID = row.Field<int>("group_id"),
                    MaterialGroup = row.Field<string>("group_name"),
                    MinStock = row.Field<decimal>("min_stock"),
                    MaxStock = row.Field<decimal>("max_stock"),
                    CurrentStock = row.Field<decimal>("current_stock"),
                    CostPrice = row.Field<decimal>("cost_price"),
                    SellingPrice = row.Field<decimal>("selling_price"),
                    IsActive = row.Field<bool>("is_active"),
                    StockStatus = row.Field<string>("stock_status")
                };
            }

            return null;
        }

        public string GenerateMaterialCode()
        {
            var query = "SELECT generate_code('VT', 'materials', 'material_code')";
            var result = ExecuteScalar(query);
            return result?.ToString() ?? "VT0001";
        }

        public bool AddMaterial(Material material)
        {
            var query = @"
                INSERT INTO materials (material_code, material_name, unit_id, material_group_id, 
                                     min_stock, max_stock, cost_price, selling_price, is_active, created_by)
                VALUES (@material_code, @material_name, @unit_id, @material_group_id,
                       @min_stock, @max_stock, @cost_price, @selling_price, @is_active, @created_by)";

            var parameters = new Dictionary<string, object>
            {
                { "@material_code", material.MaterialCode },
                { "@material_name", material.MaterialName },
                { "@unit_id", material.UnitID },
                { "@material_group_id", material.MaterialGroupID },
                { "@min_stock", material.MinStock },
                { "@max_stock", material.MaxStock },
                { "@cost_price", material.CostPrice },
                { "@selling_price", material.SellingPrice },
                { "@is_active", material.IsActive },
                { "@created_by", 1 } // TODO: Get from current user
            };

            return ExecuteNonQuery(query, parameters) > 0;
        }

        public bool UpdateMaterial(Material material)
        {
            var query = @"
                UPDATE materials 
                SET material_name = @material_name, unit_id = @unit_id, material_group_id = @material_group_id,
                    min_stock = @min_stock, max_stock = @max_stock, cost_price = @cost_price,
                    selling_price = @selling_price, is_active = @is_active
                WHERE material_id = @material_id";

            var parameters = new Dictionary<string, object>
            {
                { "@material_id", material.MaterialID },
                { "@material_name", material.MaterialName },
                { "@unit_id", material.UnitID },
                { "@material_group_id", material.MaterialGroupID },
                { "@min_stock", material.MinStock },
                { "@max_stock", material.MaxStock },
                { "@cost_price", material.CostPrice },
                { "@selling_price", material.SellingPrice },
                { "@is_active", material.IsActive }
            };

            return ExecuteNonQuery(query, parameters) > 0;
        }

        public bool DeleteMaterial(int materialID)
        {
            var query = "UPDATE materials SET is_active = false WHERE material_id = @material_id";
            var parameters = new Dictionary<string, object>
            {
                { "@material_id", materialID }
            };

            return ExecuteNonQuery(query, parameters) > 0;
        }

        public List<Material> SearchMaterials(string keyword)
        {
            var materials = new List<Material>();
            var query = @"
                SELECT * FROM vw_materials 
                WHERE material_code LIKE @keyword OR material_name LIKE @keyword
                ORDER BY material_code";

            var parameters = new Dictionary<string, object>
            {
                { "@keyword", $"%{keyword}%" }
            };

            var dataTable = ExecuteQuery(query, parameters);

            foreach (DataRow row in dataTable.Rows)
            {
                materials.Add(new Material
                {
                    MaterialID = row.Field<int>("material_id"),
                    MaterialCode = row.Field<string>("material_code"),
                    MaterialName = row.Field<string>("material_name"),
                    UnitName = row.Field<string>("unit_name"),
                    MaterialGroup = row.Field<string>("group_name"),
                    CurrentStock = row.Field<decimal>("current_stock"),
                    CostPrice = row.Field<decimal>("cost_price"),
                    SellingPrice = row.Field<decimal>("selling_price"),
                    IsActive = row.Field<bool>("is_active")
                });
            }

            return materials;
        }
    }
}