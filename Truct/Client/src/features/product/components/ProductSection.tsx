import React from "react";

interface ProductSectionProps {
  title: string;
  content?: string;
  padding?: string;
}

const ProductSection: React.FC<ProductSectionProps> = ({
  title,
  content = "",
  padding = "py-10",
}) => {
  return (
    <div className="flex justify-between items-start mb-4 w-[100%] mx-auto gap-x-4">
      <div className={`w-full bg-white ${padding}`}>
        <h2 className="font-semibold mb-2 text-gray-800 text-left text-sm px-6">
          {title}
        </h2>
        <div className="h-[1px] bg-gray-300 w-full mb-4 px-6"></div>
        <div className="w-full max-w-none border border-gray-300 rounded-md p-0 bg-white overflow-hidden text-left px-6 py-4">
           {content ? (
                <div dangerouslySetInnerHTML={{ __html: content }} />
            ) : (
                <div></div>
            )}

        </div>
      </div>
    </div>
  );
};

export default ProductSection;