import React from "react";
import { Link } from "react-router-dom";

import homeIcon from '@/assets/icons/home.png';
import { useTranslation } from "react-i18next";

interface BreadcrumbProps {
    path?: CategoryResponse[];
    name?: string;
    categoryId?: number;
    total?: number;
}
const Breadcrumb: React.FC<BreadcrumbProps> = ({ path, name, categoryId, total }) => {
    const { t, i18n } = useTranslation();
    return (
        <div className="flex justify-between items-center mb-4 w-[100%] mx-auto">
            <div className="flex items-center">
                <Link
                to="/"
                className="flex items-center text-[#009EE2] hover:text-blue-800 text-sm hover:underline"
                >
                <img src={homeIcon} alt={t("search.home")} className="h-4 w-4 mr-1" />
                <span>{t("search.home")}</span>
                </Link>

                {path && path?.length > 0 &&
                    path.map((cat) => (
                        <span key={cat.id} className="flex items-center">
                        <span className="mx-1" />
                        <span className="mx-1 text-gray-600">/</span>
                        <span className="mx-1" />
                        {categoryId === cat.id ? (
                                <span className="font-medium text-sm">{cat.name}</span>
                            ) : (
                                <Link
                                to={`/search-by-category?categoryId=${cat.id}`}
                                className="flex items-center text-[#009EE2] hover:text-blue-800 text-sm hover:underline"
                                >
                                {cat.name}
                                </Link>
                            )}
                        </span>
                    ))
                }
                {name && (
                    <span className="flex items-center">
                    <span className="mx-1" />
                    <span className="mx-1 text-gray-600">/</span>
                    <span className="mx-1" />
                    <span className="font-medium text-sm">{name}</span>
                    </span>
                )}
            </div>
            {total !== undefined && 
                (<div>{t("search.total_items", { count: total || 0 })}</div>)
            }
        </div>
    );
};

export default Breadcrumb;