import React from "react";

type PaginationProps = {
  currentPage: number;      // Trang hiện tại (bắt đầu từ 0)
  totalPage: number;        // Tổng số trang
  onPageChange: (page: number) => void; // Hàm xử lý khi chuyển trang
};

const getVisiblePages = (currentPage: number, totalPage: number): (number | string)[] => {
  const pages: (number | string)[] = [];

  if (totalPage <= 5) {
    for (let i = 0; i < totalPage; i++) pages.push(i);
  } else {
    pages.push(0);

    if (currentPage > 2) pages.push("...");

    const start = Math.max(1, currentPage - 1);
    const end = Math.min(totalPage - 2, currentPage + 1);

    for (let i = start; i <= end; i++) pages.push(i);

    if (currentPage < totalPage - 3) pages.push("...");

    pages.push(totalPage - 1);
  }

  return pages;
};

const Pagination: React.FC<PaginationProps> = ({ currentPage, totalPage, onPageChange }) => {
  console.log("abc")
  if (totalPage <= 1) return null;

  const visiblePages = getVisiblePages(currentPage, totalPage);

  return (
    <div className="flex justify-center items-center mt-6 space-x-2">
      <button
        onClick={() => onPageChange(currentPage - 1)}
        disabled={currentPage === 0}
        className="px-3 py-1 border rounded disabled:opacity-50"
      >
        이전
      </button>

      {visiblePages.map((page, index) =>
        typeof page === "number" ? (
          <button
            key={index}
            onClick={() => onPageChange(page)}
            className={`px-3 py-1 border rounded ${
              currentPage === page ? "bg-blue-500 text-white" : "bg-white"
            }`}
          >
            {page + 1}
          </button>
        ) : (
          <span key={index} className="px-3 py-1 text-gray-500">
            ...
          </span>
        )
      )}

      <button
        onClick={() => onPageChange(currentPage + 1)}
        disabled={currentPage === totalPage - 1}
        className="px-3 py-1 border rounded disabled:opacity-50"
      >
        다음
      </button>
    </div>
  );
};

export default Pagination;
