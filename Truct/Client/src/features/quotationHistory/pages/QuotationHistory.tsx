import { getQuotationDetail, quotationHistory } from "@/services/quotation.service";
import { QuotationHistoryRequest, QuotationHistoryResponse, QuotationResponse } from "@/types/Quotation";
import { getToken } from "@/utils/authUtils";
import { useEffect, useState } from "react";
import { Link } from "react-router-dom";

import homeIcon from '@/assets/icons/home.png';
import { useTranslation } from "react-i18next";

import quoteHistoryIcon from '@/assets/icons/quote_history.png';
import downloadIcon from '@/assets/icons/download.png';
import QuotationModal from "@/features/product/components/QuotationModal";
import Breadcrumb from "@/components/common/Breadcrumb";
import { PaginationMeta } from "@/types/ApiResponse";
import Pagination from "@/components/common/Pagination";

export default function QuotationHistory() {
    const token = getToken();
    if (!token) {
        window.location.href = "/login";
    }

    const [quotationHistories, setQuotationHistories] = useState<QuotationHistoryResponse[]>([]);
    const [quotationDetail, setQuotationDetail] = useState<QuotationResponse>();    
    const [showQuotationModal, setShowQuotationModal] = useState(false);
    const [loading, setLoading] = useState(false);
    const [meta, setMeta] = useState<PaginationMeta | null>(null);
    const [page, setPage] = useState(0);
    const [fromDate, setFromDate] = useState<string>("");
    const [toDate, setToDate] = useState<string>("");


    useEffect(() => {
        handleSearch();
    }, [page]);

    const handleSearch = () => {
        const fetchFilters = async () => {
            const request: QuotationHistoryRequest = {
                fromDate: fromDate ? new Date(fromDate + "T00:00:00Z").toISOString() : undefined,
                toDate: toDate ? new Date(toDate + "T23:59:59Z").toISOString() : undefined,
                page: page,
                limit: 30,
            };
            try {
                const data = await quotationHistory(request);
                setQuotationHistories(data.data);
                setMeta(data.meta)
            } catch (error) {

            }
        };
        fetchFilters();
    };


    const getDetailQuotation = async (id: number) => {
        setLoading(true);
            try {
                const result = await getQuotationDetail(id);
                setQuotationDetail(result.data);
                setShowQuotationModal(true);
            } catch (error) {
                console.error("Quotation detail error:", error);
            } finally {
                setLoading(false);
            }
        }
    console.log(meta)
    console.log(meta?.totalPage)
    return (
        <div className="p-4">
            <Breadcrumb
                name={"見積履歴"}
            />

            <div className="flex justify-between items-start mb-4 w-[100%] mx-auto"> 
                <div className="w-[100%] text-left">
                    <div className="w-[100%] mx-auto mb-4 flex items-center gap-4 justify-end">
                        <div className="flex items-center gap-2">
                            <label className="text-sm font-medium">開始日:</label>
                            <input
                            type="date"
                            value={fromDate}
                            max={toDate}
                            onChange={(e) => setFromDate(e.target.value)}
                            className="border rounded px-2 py-1 text-sm"
                            />
                        </div>
                        <div className="flex items-center gap-2">
                            <label className="text-sm font-medium">終了日:</label>
                            <input
                            type="date"
                            value={toDate}
                            min={fromDate}
                            onChange={(e) => setToDate(e.target.value)}
                            className="border rounded px-2 py-1 text-sm"
                            />
                        </div>
                        <button
                            onClick={() => handleSearch()}
                            className="bg-blue-600 text-white px-4 py-1 rounded text-sm hover:bg-blue-700"
                        >
                            検索
                        </button>
                    </div>
                    <div className="w-full bg-white  pb-10">
                        <table style={{borderCollapse: 'collapse', width: '100%' }}>
                            <thead>
                                <tr style={{ backgroundColor: '#f2f2f2' }}>
                                    <th style={{ width: '5%', border: '1px solid #ccc', padding: '10px', textAlign: 'center', fontSize: '13px' }}>番号</th>
                                    <th style={{ width: '40%', border: '1px solid #ccc', padding: '10px', textAlign: 'center', fontSize: '13px' }}>メッセージ</th>
                                    <th style={{ width: '10%', border: '1px solid #ccc', padding: '10px', textAlign: 'center', fontSize: '13px' }}>配送方法</th>
                                    <th style={{ width: '15%', border: '1px solid #ccc', padding: '10px', textAlign: 'center', fontSize: '13px' }}>合計金額(JPY)</th>
                                    <th style={{ width: '10%', border: '1px solid #ccc', padding: '10px', textAlign: 'center', fontSize: '13px' }}>作成日</th>
                                    <th style={{ width: '10%', border: '1px solid #ccc', padding: '10px', textAlign: 'center', fontSize: '13px' }}>有効期限</th>
                                    <th style={{ width: '10%', border: '1px solid #ccc', padding: '10px', textAlign: 'center', fontSize: '13px' }}>操作</th>
                                </tr>
                            </thead>
                            <tbody>
                                {quotationHistories?.map((item, index) => (
                                    <tr key={item.id}>
                                        <td style={{ border: '1px solid #ccc', padding: '10px', textAlign: 'center' }}> {index + 1}</td>
                                        <td style={{ border: '1px solid #ccc', padding: '10px', textAlign: 'left' }}>{item.message}</td>
                                        <td style={{ border: '1px solid #ccc', padding: '10px', textAlign: 'center' }}>{item.shippingMethodCode}</td>
                                        <td style={{ border: '1px solid #ccc', padding: '10px', textAlign: 'center' }}>{item.total?.toLocaleString("de-DE")}</td>
                                        <td style={{ border: '1px solid #ccc', padding: '10px', textAlign: 'center' }}>{item.createdAt}</td>
                                        <td style={{ border: '1px solid #ccc', padding: '10px', textAlign: 'center' }}>{item.expiredAt}</td>
                                        <td style={{ border: '1px solid #ccc', padding: '10px', textAlign: 'center' }}>
                                             <div style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', gap: '10px' }}>
                                                    <img
                                                        src={quoteHistoryIcon}
                                                        alt="Quote History"
                                                        style={{ width: 20, height: 20, cursor: 'pointer' }}
                                                        onClick={() => getDetailQuotation(item.id)}
                                                        />
                                                </div>
                                        </td>
                                    </tr>
                                ))}
                            </tbody>
                        </table>
                    </div>
                    {meta && meta.totalPage > 1 && (
                        <Pagination
                            currentPage={meta?.currentPage ?? 0}
                            totalPage={meta.totalPage ?? 1}
                            onPageChange={(page) => setPage(page)}
                        />
                    )}
                </div>
            </div>
            {showQuotationModal && quotationDetail && (
                <QuotationModal
                    quotation={quotationDetail}
                    onClose={() => setShowQuotationModal(false)}
                />
            )}
        </div>
    );
}