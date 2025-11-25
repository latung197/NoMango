import React, { useState } from "react";

interface ProductImageCarouselProps {
  images: string[];
}

const ProductImageCarousel: React.FC<ProductImageCarouselProps> = ({ images }) => {
  const [currentIndexImg, setCurrentIndexImg] = useState(0);

  const handlePrev = () => {
    setCurrentIndexImg((prev) => (prev === 0 ? images.length - 1 : prev - 1));
  };

  const handleNext = () => {
    setCurrentIndexImg((prev) => (prev === images.length - 1 ? 0 : prev + 1));
  };

  return (
    <div className="w-4/12 flex flex-col gap-4 mt-8">
      <div className="relative">
        <div className="relative w-full h-[400px] flex items-center justify-center bg-white">
          <img
            src={images[currentIndexImg]}
            alt={`Image ${currentIndexImg + 1}`}
            className="max-w-full max-h-full object-contain bg-white"
            style={{ backgroundColor: "white" }}
          />
        </div>
        <button
          onClick={handlePrev}
          className="absolute left-4 top-1/2 -translate-y-1/2 w-10 h-10 rounded-full bg-white border border-gray-300 hover:bg-gray-100 text-black text-2xl leading-none flex items-center justify-center shadow-lg hover:shadow-xl transition-all duration-200 hover:scale-110"
        >
          &lsaquo;
        </button>
        <button
          onClick={handleNext}
          className="absolute right-4 top-1/2 -translate-y-1/2 w-10 h-10 rounded-full bg-white border border-gray-300 hover:bg-gray-100 text-black text-2xl leading-none flex items-center justify-center shadow-lg hover:shadow-xl transition-all duration-200 hover:scale-110"
        >
          &rsaquo;
        </button>
      </div>
      <div className="thumbnails flex gap-2 mt-2">
        {images.map((img, index) => (
          <img
            key={index}
            src={img}
            alt={`Thumbnail ${index + 1}`}
            className={`w-16 h-16 object-cover cursor-pointer border ${
              index === currentIndexImg ? "ring-2 ring-blue-500" : ""
            }`}
            style={{ backgroundColor: "white" }}
            onClick={() => setCurrentIndexImg(index)}
          />
        ))}
      </div>
    </div>
  );
};

export default ProductImageCarousel;