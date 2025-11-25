import { QuotationResponse } from "@/types/Quotation";
import React, { useState } from "react";

import downloadIcon from '@/assets/icons/download_while.png';
import { quotationDownload } from "@/services/quotation.service";

type InfoRowProps = {
  label: string;
  value: string | number;
  bold?: boolean;
};

type SummaryRowProps = {
  label: string;
  value: string | number;
};


type QuotationModalProps = {
  quotation: QuotationResponse;
  onClose: () => void;
};

const QuotationModal: React.FC<QuotationModalProps> = ({ quotation, onClose }) => {
  const [loading, setLoading] = useState(false);

  const handleClick = async () => {
    try {
      setLoading(true);
      await quotationDownload(quotation.id);
    } catch (error) {
      console.error("Download failed", error);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
        <div className="bg-white rounded-lg shadow-lg w-[70%] p-6">

        <div className="text-2xl font-medium text-center mb-4">見積書</div>

        <div className="grid grid-cols-12 gap-6 mb-6">
          <div className="col-span-10">
            <InfoRow label="宛先" value={quotation.company} bold />
            <InfoRow label="住所" value={quotation.address} />
            <InfoRow label="携帯番号" value={quotation.phoneNumber} />
            <InfoRow label="メール" value={quotation.email} />
            <InfoRow label="担当者" value={`${quotation.name} 様`} />
          </div>
          <div className="col-span-2">
            <InfoRow label="日付" value={quotation.createdAt} />
            <InfoRow label="有効期限" value={quotation.expiredAt} />
          </div>
        </div>

        <div className="overflow-x-auto">
          <table className="table-auto w-full border border-gray-300">
            <thead className="bg-gray-300">
                <tr className="border border-gray-400 bg-gray-100">
                    <th className="text-center p-2 border font-medium border-gray-300">番号</th>
                    <th className="text-center p-2 border font-medium border-gray-300">商品名</th>
                    <th className="text-center p-2 border font-medium border-gray-300">品番</th>
                    <th className="text-center p-2 border font-medium border-gray-300">数量</th>
                    <th className="text-center p-2 border font-medium border-gray-300">単位</th> 
                    <th className="text-center p-2 border font-medium border-gray-300">単価</th> 
                    <th className="text-center p-2 border font-medium border-gray-300">金額(JPY)</th>   
                </tr>
            </thead>
            <tbody>
              {quotation.products.map((product, index) => (
                <tr key={index} className="border-t">
                  <td className="text-center border border-gray-300 p-2">{index + 1}</td>
                  <td className="text-center border border-gray-300 p-2">{product.name}</td>
                  <td className="text-center border border-gray-300 p-2">{product.code}</td>
                  <td className="text-center border border-gray-300 p-2">{product.quantity}</td>
                  <td className="text-center border border-gray-300 p-2">{product.unit}</td>
                  <td className="text-center border border-gray-300 p-2">{product.unitPrice?.toLocaleString("de-DE")}</td>
                  <td className="text-center border border-gray-300 p-2">{product.amount?.toLocaleString("de-DE")}</td>
                </tr>
              ))}
                <SummaryRow label="小計" value={quotation.subTotalPrice?.toLocaleString("de-DE")} />
                <SummaryRow label={`VAT (${quotation.vat}%)`} value={quotation.vatFee?.toLocaleString("de-DE")} />
                <SummaryRow label="送料" value={quotation.shippingFee?.toLocaleString("de-DE")} /> 
                <SummaryRow label="合計金額" value={quotation.total?.toLocaleString("de-DE")} /> 
            </tbody>
          </table>
        </div>

        <div className="flex justify-end items-center mt-6 space-x-4">
          <button
            onClick={onClose}
            className="flex items-center space-x-2 bg-gray-800 hover:bg-gray-700 text-white px-4 py-2 rounded-lg text-sm font-medium"
          >
            <span>閉じる</span>
          </button>
          <button
            className="flex items-center space-x-2 bg-[#44B678] hover:bg-green-600 text-white px-4 py-2 rounded-lg text-sm font-medium"
            onClick={handleClick}
            disabled={loading}
          >
            <img src={downloadIcon} alt="Download" className="w-4 h-4" />
            <span>{loading ? 'ダウンロード中...' : 'ダウンロード'}</span>
          </button>
        </div>

      </div>
    </div>
  );
};

const InfoRow = ({ label, value, bold }: InfoRowProps) => (
  <div className="flex items-center mb-2">
    <div className="w-20 font-medium text-left">{label}</div>
    <div className="mx-1">:</div>
    <div className={bold ? "font-semibold" : ""}>{value}</div>
  </div>
);

const SummaryRow = ({ label, value }: SummaryRowProps) => (
  <tr className="border-t font-medium">
    <td colSpan={6} className="text-right p-2">{label}</td>
    <td className="text-center border border-gray-300 p-2">{value}</td>
  </tr>
);

export default QuotationModal;