import { Navigate, useNavigate, useSearchParams } from "react-router-dom";
import { useState, useEffect } from 'react';
import { useTranslation } from 'react-i18next';
import { Link } from "react-router-dom";
import { useParams } from "react-router-dom";
import homeIcon from '@/assets/icons/home.png';
import { useCategory } from "@/hooks/useCategory";
import { findCategoryPath } from "@/utils/categoryUtils";
import { getProductDetail,getProductContent, getProductMeta, getListProductRelatedViews, getListProductRelatedByCategory } from "@/services/product.service";
import { AccessoryResponse, Attribute, AttributeType, ProductDetailResponse, ProductMetaResponse, ProductResponse, SkuAttrResponse } from "@/types/Product";
import AttributeRenderer from "../components/attributes/AttributeRenderer";

import warehouse from '@/assets/icons/warehouse.png';
import cartIcon from '@/assets/icons/cart.png';
import wishlist from '@/assets/icons/bookmark.png';
import wishlistWhile from '@/assets/icons/bookmark-white.png';
import noteIcon from '@/assets/icons/icon_note.png';

import ProductCard from "@/components/shared/ProductCard";
import NotFoundComponent from "@/components/errors/NotFound.component";
import { FormDataRequest, QuotationAttributeRequest, QuotationRequest, QuotationResponse } from "@/types/Quotation";
import { quotation, quotationDownload } from "@/services/quotation.service";
import InfoModal from "../components/InfoModal";
import { useUserInfo } from "@/hooks/useUserInfo";
import QuotationModal from "../components/QuotationModal";
import { getAccessoriesByProductId } from "@/services/accessories.service";
import { AddToCartRequest } from "@/types/Cart";
import { addToCart, getCartCount } from "@/services/cart.service";
import { useCart } from "@/context/CartContext";
import ProductSection from "../components/ProductSection";
import AccessoryList from "../components/AccessoryList";
import ProductImageCarousel from "../components/ProductImage";
import SkuSelector from "../components/SkuSelector";
import Breadcrumb from "@/components/common/Breadcrumb";

export default function ProductDetail() {
    const navigate = useNavigate();
    const { id } = useParams<{ id: string }>();
    const { t, i18n } = useTranslation();
    const { userInfo, setUserInfo } = useUserInfo();

    const [product, setProduct] = useState<ProductDetailResponse | null>(null);
    const [productsRelatedViews, setProductsRelatedViews] = useState<{ data: ProductResponse[]; meta: any } | null>(null);
    const [productsRelatedByCategory, setProductsRelatedByCategory] = useState<{ data: ProductResponse[]; meta: any } | null>(null);
    const [productContentDescription, setProductContentDescription] = useState<string | null>(null);
    const [productContentBasicInfo, setProductContentBasicInfo] = useState<string | null>(null);
    const [productContentNote, setProductContentNote] = useState<string | null>(null);
    const [productContentSpecification, setProductContentSpecification] = useState<string | null>(null);

    const [productMeta, setProductMeta] = useState<ProductMetaResponse>();
    const [accessories, setAccessories] = useState<AccessoryResponse[]>([]);
    const [selectedShippingMethod, setSelectedShippingMethod] = useState<number | null>(null);
    const [isShippingMethodInvalid, setIsShippingMethodInvalid] = useState(false);
    const [isSkuInvalid, setIsSkuInvalid] = useState(false);

    const [selectedSku, setSelectedSku] = useState<number | null>(null);

    const [images, setImages] = useState<string[]>([]);
    const [quantity, setQuantity] = useState<number>(1);

    const [attributeValues, setAttributeValues] = useState<Record<number, string | number | null>>({});
    const [invalidAttrIds, setInvalidAttrIds] = useState<number[]>([]);
    const [attributeErrors, setAttributeErrors] = useState<Record<number, boolean>>({});

    const { categories } = useCategory();
    const path = findCategoryPath(categories, Number(product?.categoryId)) || [];

    const [showInfoModal, setShowInfoModal] = useState(false);
    const [form, setForm] = useState<FormDataRequest>();

    const [quotationData, setQuotationData] = useState<QuotationResponse>();
    const [showQuotationModal, setShowQuotationModal] = useState(false);

    const [loading, setLoading] = useState(false);
    const [loadingAddCart, setLoadingAddCart] = useState(false);
    const { setCartCount } = useCart();
    const [showSuccessModal, setShowSuccessModal] = useState(false);

    const [errors, setErrors] = useState<Record<string, string>>({});
    const validateForm = (form: FormDataRequest): Record<string, string> => {
        const errors: Record<string, string> = {};
        if (userInfo) {
            return errors;
        }
        if (!form.name?.trim()) errors.name = t('message.validate.required_input');
        if (!form.email?.trim()) errors.email = t('message.validate.required_input');
        if (!form.phoneNumber?.trim()) errors.phoneNumber = t('message.validate.required_input');
        if (!form.company?.trim()) errors.company = t('message.validate.required_input');
        if (!form.address?.trim()) errors.address = t('message.validate.required_input');
        return errors;
    };
    const resetForm = () => {
        setForm({
            name: '',
            email: '',
            phoneNumber: '',
            company: '',
            position: '',
            address: '',
            message: '',
        });
    };

    useEffect(() => {
        if (!Number(id)) return;
        const fetchFilters = async () => {
            try {
                const data = await getProductDetail(Number(id));
                setProduct(data);
                setImages(data?.imageUrls ?? []);
            } catch (error) {

            }
        };
        fetchFilters();
    }, [id]);

    useEffect(() => {
        if (!Number(id)) return;
        const fetchFilters = async () => {
            try {
                const data = await getProductMeta(Number(id));
                setProductMeta(data);
            } catch (error) {

            }
        };
        fetchFilters();
    }, [id]);

    useEffect(() => {
        if (!Number(id)) return;
        const fetchFilters = async () => {
            try {
                const data = await getProductContent(Number(id), "DETAIL_DESCRIPTION");
                setProductContentDescription(data?.htmlContent);
            } catch (error) {
            }
        };
        fetchFilters();
    }, [id]);

    useEffect(() => {
        if (!Number(id)) return;
        const fetchFilters = async () => {
            try {
                const data = await getProductContent(Number(id), "BASIC_INFO");
                setProductContentBasicInfo(data?.htmlContent);
            } catch (error) {
            }
        };
        fetchFilters();
    }, [id]);

    useEffect(() => {
        if (!Number(id)) return;
        const fetchFilters = async () => {
            try {
                const data = await getProductContent(Number(id), "NOTE");
                setProductContentNote(data?.htmlContent);
            } catch (error) {
            }
        };
        fetchFilters();
    }, [id]);

    useEffect(() => {
        if (!Number(id)) return;
        const fetchFilters = async () => {
            try {
                const data = await getProductContent(Number(id), "SPECIFICATION");
                setProductContentSpecification(data?.htmlContent);
            } catch (error) {
            }
        };
        fetchFilters();
    }, [id]);

    useEffect(() => {
        if (!Number(id)) return;
        const fetchFilters = async () => {
            try {
                const data = await getAccessoriesByProductId(Number(id));
                setAccessories(data.data);
            } catch (error) {
            }
        };
        fetchFilters();
    }, [id]);

    const handleAttributeChange = (
        attrProductId: number,
        value: string | number | null,
        attr: Attribute
        ) => {
        setAttributeValues((prev) => {
            const groupAttrProductIds = attr.options?.length && attr.type.startsWith('SELECT')
                    ? attr.options.map((opt) => opt.productAttributeId)
                    : attr.productAttributeId !== null
                    ? [attr.productAttributeId]
                    : [];

            const cleaned = { ...prev };
            groupAttrProductIds.forEach((id) => {
                delete cleaned[id];
            });
            if (value === null) {
                return cleaned;
            }
            const updated = {
            ...cleaned,
            [attrProductId]: value,
            };
            
            const hasValue = groupAttrProductIds.some((id) => {
                const val = updated[id];
                return val !== null && val !== undefined && val !== '';
            });
            setInvalidAttrIds((prevInvalids) => {
                const isAlreadyInvalid = prevInvalids.includes(attr.attributeId);

                if (hasValue && isAlreadyInvalid) {
                    return prevInvalids.filter((id) => id !== attr.attributeId);
                }
                if (!hasValue && !isAlreadyInvalid) {
                    return [...prevInvalids, attr.attributeId];
                }
                return prevInvalids;
            });

            return updated;
        });
    };

    const validateShippingMethod = () => {
        const isValid = selectedShippingMethod !== null;
        setIsShippingMethodInvalid(!isValid);
        return isValid;
    };

    const validateSku = () => {
        const isValid = selectedSku !== null;
        setIsSkuInvalid(!isValid);
        return isValid;
    };

    const validateAttributes = (attributes: Attribute[], attributeValues: Record<number, string | number | null>) => {
        const invalidIds: number[] = [];
        for (const attr of attributes) {
            if (
                attr.type === AttributeType.FIX_NUMBER ||
                attr.type === AttributeType.FIX_TEXT
            ) {
                continue;
            }
            const productIds =
                attr.type.startsWith('SELECT') && attr.options
                    ? attr.options.map((opt) => opt.productAttributeId)
                    : attr.productAttributeId !== null
                    ? [attr.productAttributeId]
                    : [];

                const hasValue = productIds.some((id) => {
                const val = attributeValues[id];
                return val !== null && val !== undefined && val !== '';
            });
            if (!hasValue) {
                
                invalidIds.push(attr.attributeId);
            }
        }
        return invalidIds;
    };

    useEffect(() => {
         if (!Number(id)) return;
        const fetchProducts = async () => {
            const response = await getListProductRelatedViews(Number(id));
            setProductsRelatedViews(response);
        };
        fetchProducts();
    }, []);

    useEffect(() => {
         if (!Number(id)) return;
        const fetchProducts = async () => {
            const response = await getListProductRelatedByCategory(Number(id));
            setProductsRelatedByCategory(response);
        };
        fetchProducts();
    }, []);

    const attributesPayload: QuotationAttributeRequest[] = Object.entries(attributeValues).map(
        ([productAttributeId, value]) => ({
            productAttributeId: Number(productAttributeId),
            valueNumber: typeof value === 'number' ? value : null,
            valueText: typeof value === 'string' ? value : null,
        })
    );

    const handleClick = async () => {
        const isShippingValid = validateShippingMethod();

        let invalids: number[] = [];
        if (productMeta?.attributes) {
            invalids = validateAttributes(productMeta.attributes, attributeValues);
            setInvalidAttrIds(invalids);
        }
        const hasAttributeError = Object.values(attributeErrors).some((err) => err === true);
        if (!isShippingValid || invalids.length > 0 || hasAttributeError) {

            return;
        }
        resetForm();
        setShowInfoModal(true);
    
    };

    const handleClickAddToCart = async () => {
        const isSkuValid = validateSku();
        let invalids: number[] = [];
        if (productMeta?.attributes) {
            invalids = validateAttributes(productMeta.attributes, attributeValues);
            setInvalidAttrIds(invalids);
        }
        const hasAttributeError = Object.values(attributeErrors).some((err) => err === true);
        if (!isSkuValid || invalids.length > 0 || hasAttributeError) {
            return;
        }
        setLoadingAddCart(true);
        const requestData: AddToCartRequest = {
            productId: Number(id),
            quantity: quantity,
            attributes: attributesPayload
        };
        try {
            const result = await addToCart(requestData);
            setShowSuccessModal(true);
            const count = await getCartCount();
            setCartCount(count);
        } finally {
            setLoadingAddCart(false);
        }
    };

    const handleGetQuotation = async () => {
        setLoading(true);
        const requestData: QuotationRequest = {
            productId: Number(id),
            name: form?.name ?? null,
            company: form?.company ?? null,
            position: form?.position ?? null,
            address: form?.address ?? null,
            email: form?.email ?? null,
            phoneNumber: form?.phoneNumber ?? null,
            message: form?.message ?? null,
            quantity: quantity,
            shippingMethodId: selectedShippingMethod,
            attributes: attributesPayload
        };
        try {
            const result = await quotation(requestData);
            setQuotationData(result.data);
            setShowQuotationModal(true);
            
        } catch (error) {
            console.error("Quotation error:", error);
        } finally {
            setLoading(false);
            setShowInfoModal(false)
        }
    }

    const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
        const { name, value } = e.target;
        setForm((prev) => ({ ...prev, [name]: value }));
        setErrors((prevErrors) => {
                    const newErrors = { ...prevErrors };
                    if (newErrors[name] && value.trim() !== '') {
                        delete newErrors[name];
                    }
                    return newErrors;
                });
    };

    useEffect(() => {
        if (productMeta?.skus) {
            for (const sku of productMeta?.skus) {
                const isMatch = sku.productAttributeIds.every((productAttrId) => {
                    const value = attributeValues[productAttrId];
                    return value !== null && value !== undefined && value !== '';
                });

                if (isMatch) {
                    setSelectedSku(sku.skuId);
                    setIsSkuInvalid(false);
                    return;
                }
            }
        }
        setSelectedSku(null);
    }, [attributeValues]);

    return (
        <div>
            {!product && (
                <NotFoundComponent/>
            )}
            {product && (
                <div className="p-4">
                    <Breadcrumb
                        path={path}
                        name={product?.name ?? ""}
                    />
                    <div className="flex justify-between items-start mb-4 w-[100%] mx-auto gap-x-4">
                        <div className="w-[20%] ">
                            <div className="text-left border border-gray-300 rounded max-h-screen overflow-y-auto">
                                <div className="bg-[#02CECF] text-white text-center px-4 py-2 text-sm font-medium w-full sticky top-0">
                                    絞り込み条件
                                </div>
                                <div className="mb-4 px-4 my-4">
                                    {productMeta?.shippingMethods && productMeta?.shippingMethods.length > 0 && (
                                        <div className="mt-4">
                                            <h3 className="font-semibold mb-2 text-sm">配送方法</h3>

                                            <div
                                                className={`border rounded p-4 space-y-2 text-xs ${
                                                isShippingMethodInvalid ? 'border-red-500' : 'border-gray-300'
                                            }`}
                                            >
                                                {productMeta.shippingMethods
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
                                    {/* {productMeta && (
                                        <SkuSelector
                                            shippingAttribute={productMeta}
                                            selectedSku={selectedSku}
                                            setSelectedSku={setSelectedSku}
                                            isSkuInvalid={isSkuInvalid}
                                            setIsSkuInvalid={setIsSkuInvalid}
                                            setAttributeValues={setAttributeValues}
                                            invalidAttrIds={invalidAttrIds}
                                            setInvalidAttrIds={setInvalidAttrIds}
                                        />
                                    )} */}

                                    {productMeta?.attributes.map((attr) => (
                                        <div key={attr.attributeId} className="mt-4">
                                            <h3 className="font-semibold mb-2 text-sm">{attr.name}{attr.code ? `(${attr.code})` : ''}{attr.unit && <span className="text-gray-500">({attr.unit})</span>}</h3>
                                            <AttributeRenderer
                                                attr={attr}
                                                attributeValues={attributeValues}
                                                onChange={(attrProductId, val) => handleAttributeChange(attrProductId, val, attr)}
                                                isInvalid={invalidAttrIds.includes(Number(attr.attributeId))}
                                                reportError={(hasError: boolean) =>
                                                    setAttributeErrors((prev) => ({
                                                    ...prev,
                                                    [attr.attributeId]: hasError,
                                                    }))
                                                }
                                            />
                                        </div>
                                    ))}
                                </div>
                            </div>
                        </div>
                        <div className="w-[80%] text-left">
                            <div className="flex w-full">
                                <ProductImageCarousel images={images} />
                                <div className="w-8/12 mb-2 mt-12">
                                    <h2 className="text-xl font-bold">{product?.name}</h2>

                                    <div className="flex items-center space-x-2 text-sm mt-4">
                                        <img src={warehouse} alt="Lead time" className="w-5 h-5"/>
                                        <span>{product?.leadTimeDays} 日</span>
                                    </div>
                                    <div className="flex items-center space-x-2 mt-4">
                                        <label htmlFor="quantity" className="text-sm font-medium text-gray-700">数量:</label>
                                        <div className="flex flex-col">
                                            <input
                                                id="quantity"
                                                type="number"
                                                min={1}
                                                max={99999}
                                                step={1}
                                                className={`w-[70px] border border-gray-300 rounded px-3 pr-0 py-1 text-sm text-righ`}
                                                value={quantity}
                                                onKeyDown={(e) => {
                                                    if (e.key === "." || e.key === ",") {
                                                    e.preventDefault();
                                                    }
                                                }}
                                                onChange={(e) => {
                                                    let val = Math.floor(Number(e.target.value));
                                                    if (isNaN(val) || val < 1) val = 1;
                                                    if (val > 99999) val = 99999; 
                                                    setQuantity(val < 1 ? 1 : val);
                                                }}
                                            />
                                        </div>
                                    </div>

                                    <div className="flex space-x-4 mt-4">
                                        <button className="w-28 bg-[#00B0F0] hover:bg-blue-600 text-white px-4 py-2 rounded-lg text-sm font-medium text-center"
                                            onClick={() => handleClick()}
                                            >
                                            <span>見積依頼</span>
                                        </button>
                                        <button
                                            onClick={() => handleClickAddToCart()}
                                            disabled={loadingAddCart}
                                            className={`flex items-center space-x-2 px-4 py-2 rounded-lg text-sm font-medium
                                                ${loadingAddCart ? 'bg-gray-400 text-white cursor-not-allowed' : 'bg-[#FFC74F] hover:bg-yellow-500 text-white'}`}
                                        >
                                            <img src={cartIcon} alt="Cart" className="w-4 h-4" />
                                            <span>{loadingAddCart ? '追加中...' : 'カートに追加'}</span>
                                        </button>
                                    </div>
                                    
                                    <div className="border border-gray-300 rounded-md overflow-hidden mt-8">
                                        <div className="bg-gray-300 text-black px-4 py-2 text-sm font-semibold">
                                            よく一緒に見られている商品
                                        </div>
                                        <div className="p-4 space-y-4">
                                            <div className="grid grid-cols-3 gap-2">
                                                {productsRelatedViews?.data.map((product, index) => (
                                                    <ProductCard
                                                        key={index}
                                                        item={product}
                                                        wishlistIcon={wishlist}
                                                        wishlistWhileIcon={wishlistWhile}
                                                        warehouseIcon={warehouse}
                                                        onToggleWishlist={(id) => {
                                                            setProductsRelatedViews((prev) => {
                                                            if (!prev) return prev;
                                                            return {
                                                                ...prev,
                                                                data: prev.data.map((p) =>
                                                                p.id === id ? { ...p, isWishlist: !p.isWishlist } : p
                                                                ),
                                                            };
                                                            });
                                                        }}
                                                    />
                                                ))}
                                            </div>
                                        </div>
                                    </div>
                                </div>   
                            </div>
                            <div className="relative border border-yellow-300 bg-yellow-50 rounded-md p-4 pt-10 text-sm text-gray-800">
                                <div className="flex items-center absolute top-2 left-2 bg-yellow-300 px-2 py-1 text-xs font-semibold rounded">
                                        <img src={noteIcon} alt="" className="w-4 h-4" />
                                        <span>ご注意</span>
                                </div>
                                <div>
                                    <div dangerouslySetInnerHTML={{ __html: productContentNote ?? '' }} />
                                </div>
                            </div>
                        </div>
                    </div>
                    {productContentSpecification && (
                        <ProductSection
                            title="製品仕様"
                            content={productContentSpecification ?? ''}
                            padding="py-10"
                        />
                    )}
                    
                    {productContentDescription && (
                        <ProductSection
                            title="製品詳細"
                            content={productContentDescription ?? ''}
                            padding="pb-10"
                        />
                    )}
                    {productContentBasicInfo && (
                        <ProductSection
                            title="基本情報"
                            content={productContentBasicInfo ?? ''}
                            padding="pb-10"
                        />
                    )}

                    {accessories && accessories.length > 0 && (
                         <AccessoryList accessories={accessories} />
                    )}
                   
                    <div className="flex justify-between items-start mb-4 w-[100%] mx-auto gap-x-4">
                        <div className="w-full bg-white pb-10">
                            <div className="border border-gray-300 rounded-md overflow-hidden mt-8 ">
                                <div className="bg-gray-300 text-black px-4 py-2 text-sm font-semibold text-left">
                                こちらもおすすめです
                                </div>
                                <div className="p-4 space-y-4">
                                    <div className="grid grid-cols-6 gap-3">
                                        {productsRelatedByCategory?.data.map((product, index) => (
                                            <ProductCard
                                            key={index}
                                            item={product}
                                            wishlistIcon={wishlist}
                                            wishlistWhileIcon={wishlistWhile}
                                            warehouseIcon={warehouse}
                                            onToggleWishlist={(id) => {
                                                setProductsRelatedByCategory((prev) => {
                                                if (!prev) return prev;
                                                return {
                                                    ...prev,
                                                    data: prev.data.map((p) =>
                                                    p.id === id ? { ...p, isWishlist: !p.isWishlist } : p
                                                    ),
                                                };
                                                });
                                            }}
                                            />
                                        ))}
                                    </div>
                                </div>
                            </div>
                        </div>                
                    </div>
                </div>
            )} 
            <InfoModal
                visible={showInfoModal}
                loading={loading}
                onClose={() => setShowInfoModal(false)}
                onSubmit={(data) => {
                    const validationErrors = validateForm(data);
                    if (Object.keys(validationErrors).length > 0) {
                        setErrors(validationErrors);
                        return;
                    }
                    handleGetQuotation();
                }}
                form={form}
                errors={errors}
                handleChange={handleChange}
            />

            {showQuotationModal && quotationData && (
                <QuotationModal
                    quotation={quotationData}
                    onClose={() => setShowQuotationModal(false)}
                />
            )}

            {showSuccessModal && (
                <div className="fixed inset-0 bg-black bg-opacity-30 flex items-center justify-center z-50">
                    <div className="bg-white rounded-lg shadow-lg p-6 w-80 text-center">
                    <p className="text-sm mb-4">商品をカートに追加しました。</p>
                    <div className="flex justify-center gap-4">
                        <button
                        onClick={() => setShowSuccessModal(false)}
                        className="px-4 py-2 bg-gray-300 text-gray-700 rounded hover:bg-gray-400 text-sm"
                        >
                        閉じる
                        </button>
                        <button
                        onClick={() => navigate('/user/cart')}
                        className="px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700 text-sm"
                        >
                        カートを見る
                        </button>
                    </div>
                    </div>
                </div>
            )}
        </div>   
    );
}
