import React from "react";

export interface AccessoryItem {
  id: string | number;
  name: string;
  quantity: number;
  thumbnailUrl?: string;
}

interface AccessoryListProps {
  accessories: AccessoryItem[];
}

const AccessoryList: React.FC<AccessoryListProps> = ({ accessories }) => {
  return (
    <div className="flex justify-between items-start mb-4 w-[100%] mx-auto gap-x-4">
      <div className="w-full bg-white pb-10">
        <h2 className="font-semibold mb-2 text-gray-800 text-left text-sm px-6">
          付属品一覧
        </h2>
        <div className="h-[1px] bg-gray-300 w-full mb-4 px-6"></div>
        <table style={{ width: "50%", borderCollapse: "collapse" }}>
          <thead>
            <tr style={{ backgroundColor: "#f2f2f2" }}>
              <th style={{ border: "1px solid #ccc", padding: "10px", textAlign: "center" }}>項目</th>
              <th style={{ border: "1px solid #ccc", padding: "10px", textAlign: "center" }}>画像</th>
              <th style={{ border: "1px solid #ccc", padding: "10px", textAlign: "center" }}>名称</th>
              <th style={{ border: "1px solid #ccc", padding: "10px", textAlign: "center" }}>数量</th>
            </tr>
          </thead>
          <tbody>
            {accessories.map((item, index) => (
              <tr key={item.id}>
                <td style={{ border: "1px solid #ccc", padding: "10px", textAlign: "center" }}>{index + 1}</td>
                <td style={{ border: "1px solid #ccc", padding: "10px", textAlign: "center" }}>
                  {item.thumbnailUrl && (
                    <img src={item.thumbnailUrl} alt={item.name} className="h-auto max-h-20 mx-auto" />
                  )}
                </td>
                <td style={{ border: "1px solid #ccc", padding: "10px", textAlign: "left" }}>{item.name}</td>
                <td style={{ border: "1px solid #ccc", padding: "10px", textAlign: "center" }}>{item.quantity}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
};

export default AccessoryList;