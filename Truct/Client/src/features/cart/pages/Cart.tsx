import { Link } from "react-router-dom";

import homeIcon from '@/assets/icons/home.png';
import { getToken } from "@/utils/authUtils";
import { useTranslation } from "react-i18next";
import { useEffect, useState } from "react";
import { CartResponse } from "@/types/Cart";
import { deleteById, getCartCount, getListCarts } from "@/services/cart.service";
import { ShippingMethod } from "@/types/Shipping";
import { findAllShippingMethods } from "@/services/shipping.service";

import removeIcon from '@/assets/icons/remove.png';
import { useCart } from "@/context/CartContext";
import { QuotationFromCartRequest, QuotationResponse } from "@/types/Quotation";
import { quotationFromCart } from "@/services/quotation.service";
import QuotationModal from "@/features/product/components/QuotationModal";
import Breadcrumb from "@/components/common/Breadcrumb";

export default function Cart() {
    const token = getToken();
    if (!token) {
        window.location.href = "/login";
    }
    const { t, i18n } = useTranslation();
    const [listCarts, setListCarts] = useState<CartResponse[]>();
    const [shippingMethods, setShippingMethods] = useState<ShippingMethod[]>();
    const [selectedShippingMethod, setSelectedShippingMethod] = useState<number | null>(null);
    const [isShippingMethodInvalid, setIsShippingMethodInvalid] = useState(false);
    const [message, setMessage] = useState('');
    const [selectedItems, setSelectedItems] = useState<number[]>([]);

    const [showConfirmModal, setShowConfirmModal] = useState(false);
    const [itemToDelete, setItemToDelete] = useState<number | null>(null);
    const { cartCount, setCartCount } = useCart();
    const [loading, setLoading] = useState(false);

    const [quotationData, setQuotationData] = useState<QuotationResponse>();
    const [showQuotationModal, setShowQuotationModal] = useState(false);
    const [notiSelectProductModal, setNotiSelectProductModal] = useState(false);

    useEffect(() => {
        const fetchListCarts = async () => {
            try {
                const data = await getListCarts();
                setListCarts(data.data);
            } finally {

            }
        };
        fetchListCarts();
    }, []);

    useEffect(() => {
        const fetchAllShippingMethods = async () => {
            try {
                const data = await findAllShippingMethods();
                setShippingMethods(data);
            } finally {

            }
        };
        fetchAllShippingMethods();
    }, []);

    function toggleItem(id: number) {
        setSelectedItems((prev) =>
            prev.includes(id) ? prev.filter((i) => i !== id) : [...prev, id]
        );
    }

    function handleDelete(id: number) {
        const fetchDeleteById = async () => {
            try {
                const data = await deleteById(id);
                setListCarts((prev) => {
                    if (!prev) return []; 
                    return prev.filter((item) => item.id !== id);
                });
                const count = await getCartCount();
                setCartCount(count);
            } finally {

            }
        };
        fetchDeleteById();
    }

    const handleGetQuotation = async () => {
        if (!selectedItems || selectedItems.length === 0) {
            setNotiSelectProductModal(true);
            return;
        }
        const isShippingValid = validateShippingMethod();
        if (!isShippingValid) {
            return;
        }

        setLoading(true);
        const requestData: QuotationFromCartRequest = {
            message: message ?? null,
            shippingMethodId: selectedShippingMethod,
            cartIds: selectedItems
        };
        try {
            
            const result = await quotationFromCart(requestData);
            setQuotationData(result.data);
            setShowQuotationModal(true);

            const count = await getCartCount();
            setCartCount(count);
            const data = await getListCarts();
            setListCarts(data.data);

            setSelectedShippingMethod(null);
            setMessage('');
        } catch (error) {
            console.error("Quotation from cart error:", error);
        } finally {
            setLoading(false);
        }
    }

    const validateShippingMethod = () => {
        const isValid = selectedShippingMethod !== null;
        setIsShippingMethodInvalid(!isValid);
        return isValid;
    };

    return (
        <div className="p-4">
            <Breadcrumb
                name={"カート"}
            />
            <div className="flex justify-between items-start mb-4 w-[100%] mx-auto">
                <div className="w-[75%] text-left">
                    <div className="w-full bg-white pr-8 pb-10">
                        <table style={{borderCollapse: 'collapse', width: '100%' }}>
                            <thead>
                                <tr style={{ backgroundColor: '#f2f2f2' }}>
                                    <th style={{ width: '5%', border: '1px solid #ccc', padding: '10px', textAlign: 'center', fontSize: '12px' }}></th>
                                    <th style={{ width: '5%', border: '1px solid #ccc', padding: '10px', textAlign: 'center', fontSize: '13px' }}>番号</th>
                                    <th style={{ width: '20%', border: '1px solid #ccc', padding: '10px', textAlign: 'center', fontSize: '13px' }}>画像</th>
                                    <th style={{ width: '30%', border: '1px solid #ccc', padding: '10px', textAlign: 'center', fontSize: '13px' }}>商品名</th>
                                    <th style={{ width: '30%', border: '1px solid #ccc', padding: '10px', textAlign: 'center', fontSize: '13px' }}>品番</th>
                                    <th style={{ width: '5%', border: '1px solid #ccc', padding: '10px', textAlign: 'center', fontSize: '13px' }}>数量</th>
                                    <th style={{ width: '5%', border: '1px solid #ccc', padding: '10px', textAlign: 'center', fontSize: '13px'}}>操作</th>
                                </tr>
                            </thead>
                            <tbody>
                                {listCarts?.map((item, index) => (
                                    <tr key={item.id}>
                                        <td style={{ border: '1px solid #ccc', padding: '10px', textAlign: 'center' }}>
                                            <input type="checkbox" checked={selectedItems.includes(item.id)} onChange={() => toggleItem(item.id)} />
                                        </td>
                                        <td style={{ border: '1px solid #ccc', padding: '10px', textAlign: 'center' }}> {index + 1}</td>
                                        <td style={{ border: '1px solid #ccc', padding: '10px', textAlign: 'center' }}>
                                            {item.thumbnailUrl && <img src={item.thumbnailUrl} alt={item.productName} className="w-36 h-36 object-contain mx-auto" />}
                                        </td>
                                        <td style={{ border: '1px solid #ccc', padding: '10px', textAlign: 'left' }}>{item.productName}</td>
                                        <td style={{ border: '1px solid #ccc', padding: '10px', textAlign: 'left' }}>{item.code}</td>
                                        <td style={{ border: '1px solid #ccc', padding: '10px', textAlign: 'center' }}>{item.quantity}</td>
                                        <td style={{ border: '1px solid #ccc', padding: '10px', textAlign: 'center' }}>
                                            <button
                                                onClick={() => {
                                                    setItemToDelete(item.id);
                                                    setShowConfirmModal(true);
                                                }}
                                                className="text-red-500 hover:text-red-700"
                                                >
                                                <img src={removeIcon} alt="削除" className="w-4 h-4 inline-block" />
                                            </button>
                                        </td>
                                    </tr>
                                ))}
                            </tbody>
                        </table>
                    </div>
                </div>
                <div className="w-[25%] ">
                    <div className="text-left border border-gray-300 rounded max-h-screen overflow-y-auto">
                        <div className="bg-[#02CECF] text-white text-center px-4 py-2 text-sm font-medium w-full sticky top-0">
                            見積
                        </div>
                        <div className="mb-4 px-4 my-4">
                            {shippingMethods && shippingMethods.length > 0 && (
                                <div className="mt-4">
                                    <label className="block text-sm font-semibold text-gray-700 text-left mt-4">配送方法</label>
                                    <div
                                        className={`border rounded p-4 space-y-2 text-xs mt-2 ${
                                            isShippingMethodInvalid ? 'border-red-500' : 'border-gray-300'
                                        }`}
                                    >
                                        {shippingMethods
                                            .filter((method) =>
                                            selectedShippingMethod === null || selectedShippingMethod === method.id
                                            )
                                            .map((method) => (
                                            <label key={method.id} className="flex items-center space-x-2">
                                                <input
                                                    type="radio"
                                                    name="shippingMethod"
                                                    className="mr-2"
                                                    checked={selectedShippingMethod === method.id}
                                                    onClick={() => {
                                                        if (selectedShippingMethod === method.id) {
                                                            setSelectedShippingMethod(null);
                                                            return;
                                                        }
                                                        setSelectedShippingMethod(method.id);
                                                        setIsShippingMethodInvalid(false);
                                                    }}
                                                />
                                                {method.code}
                                                <span
                                                className="text-[11px] text-gray-500 truncate max-w-[210px] inline-block"
                                                title={method.description}
                                                >
                                                ({method.description})
                                                </span>
                                            </label>
                                            ))}

                                            {selectedShippingMethod !== null && (
                                                <div className="text-right mt-2">
                                                <button
                                                    onClick={() => setSelectedShippingMethod(null)}
                                                    className="text-blue-600 text-xs underline hover:text-blue-800"
                                                >
                                                    選択を解除する
                                                </button>
                                                </div>
                                            )}
                                        </div>
                                        {isShippingMethodInvalid && (
                                            <div className="text-red-500 text-[10px] mt-1 ml-1">{"値を選択してください"}</div>
                                        )}
                                </div>
                            )}
                            <div>
                                <label className="block text-sm font-semibold text-gray-700 text-left mt-4">メッセージ</label>
                                <textarea
                                    name="message"
                                    value={message}
                                    onChange={(e) => setMessage(e.target.value)}
                                    className={`mt-1 block w-full border rounded px-3 py-2 text-sm resize-none h-36 focus:outline-none focus:ring-1 ${
                                    'border-gray-300 focus:ring-blue-500'}`}
                                    maxLength={500}
                                />
                            </div>
                            <div className="flex justify-center py-2 mt-2">
                                <button
                                    className={`w-28 px-4 py-2 rounded-lg text-sm font-medium text-center text-white ${
                                    loading ? 'bg-gray-400 cursor-not-allowed' : 'bg-[#00B0F0] hover:bg-blue-600'
                                    }`}
                                    onClick={handleGetQuotation}
                                    disabled={loading}
                                >
                                    {loading ? (
                                    <span className="flex items-center justify-center gap-2">
                                        <svg
                                        className="animate-spin h-4 w-4 text-white"
                                        xmlns="http://www.w3.org/2000/svg"
                                        fill="none"
                                        viewBox="0 0 24 24"
                                        >
                                        <circle
                                            className="opacity-25"
                                            cx="12"
                                            cy="12"
                                            r="10"
                                            stroke="currentColor"
                                            strokeWidth="4"
                                        ></circle>
                                        <path
                                            className="opacity-75"
                                            fill="currentColor"
                                            d="M4 12a8 8 0 018-8v4l3-3-3-3v4a8 8 0 00-8 8z"
                                        ></path>
                                        </svg>
                                        <span>送信中…</span>
                                    </span>
                                    ) : (
                                    <span>見積依頼</span>
                                    )}
                                </button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            {showConfirmModal && (
                <div className="fixed inset-0 bg-black bg-opacity-30 flex items-center justify-center z-50">
                    <div className="bg-white rounded-lg shadow-lg p-6 w-80 text-center">
                    <p className="text-sm mb-4">本当に削除しますか？</p>
                    <div className="flex justify-center gap-4">
                        <button
                        onClick={() => {
                            if (itemToDelete !== null) {
                            handleDelete(itemToDelete);
                            }
                            setShowConfirmModal(false);
                            setItemToDelete(null);
                        }}
                        className="px-4 py-2 bg-red-500 text-white rounded hover:bg-red-600 text-sm"
                        >
                            削除する
                        </button>
                        <button
                            onClick={() => {
                                setShowConfirmModal(false);
                                setItemToDelete(null);
                            }}
                            className="px-4 py-2 bg-gray-300 text-gray-700 rounded hover:bg-gray-400 text-sm"
                        >
                            キャンセル
                        </button>
                    </div>
                    </div>
                </div>
            )}
            {showQuotationModal && quotationData && (
                <QuotationModal
                    quotation={quotationData}
                    onClose={() => setShowQuotationModal(false)}
                />
            )}
            {notiSelectProductModal && (
                <div className="fixed inset-0 bg-black bg-opacity-30 flex items-center justify-center z-50">
                    <div className="bg-white rounded-lg shadow-lg p-6 w-80 text-center">
                    <p className="text-sm mb-4">商品を選択してください</p>
                    <div className="flex justify-center gap-4">
                        <button
                        onClick={() => setNotiSelectProductModal(false)}
                        className="px-4 py-2 bg-gray-300 text-gray-700 rounded hover:bg-gray-400 text-sm"
                        >
                        閉じる
                        </button>
                    </div>
                    </div>
                </div>
                )}
        </div>
    );
}