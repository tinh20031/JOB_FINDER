using System;

namespace JOB_FINDER_API.Services
{
    public class ApplyPercentageCalculator
    {
        /// <summary>
        /// Tính toán phần trăm ứng tuyển dựa trên số lượt xem và số lượt apply
        /// </summary>
        /// <param name="viewCount">Số lượt xem job</param>
        /// <param name="applyCount">Số lượt ứng tuyển job</param>
        /// <param name="applyPercentage">Kết quả phần trăm apply</param>
        /// <param name="errorMessage">Thông báo lỗi nếu dữ liệu không hợp lệ</param>
        /// <returns>true nếu tính toán thành công, false nếu dữ liệu không hợp lệ</returns>
        public bool CalculateApplyPercentage(
            int viewCount,
            int applyCount,
            out double applyPercentage,
            out string errorMessage)
        {
            // Khởi tạo giá trị mặc định
            applyPercentage = 0;
            errorMessage = string.Empty;

            // Kiểm tra tính hợp lệ của dữ liệu
            if (viewCount < 0 || applyCount < 0)
            {
                errorMessage = "Dữ liệu không hợp lệ";
                return false;
            }

            // Trường hợp viewCount = 0
            if (viewCount == 0)
            {
                applyPercentage = 0;
                return true;
            }

            // Trường hợp có nhiều apply hơn view (có thể xảy ra khi dữ liệu không đồng bộ)
            if (applyCount > viewCount)
            {
                applyPercentage = 100;
                return true;
            }

            // Tính toán phần trăm apply
            applyPercentage = (double)applyCount / viewCount * 100;

            return true;
        }

        /// <summary>
        /// Tính toán phần trăm ứng tuyển và làm tròn đến số chữ số thập phân chỉ định
        /// </summary>
        /// <param name="viewCount">Số lượt xem job</param>
        /// <param name="applyCount">Số lượt ứng tuyển job</param>
        /// <param name="decimalPlaces">Số chữ số thập phân cần làm tròn</param>
        /// <param name="applyPercentage">Kết quả phần trăm apply đã làm tròn</param>
        /// <param name="errorMessage">Thông báo lỗi nếu dữ liệu không hợp lệ</param>
        /// <returns>true nếu tính toán thành công, false nếu dữ liệu không hợp lệ</returns>
        public bool CalculateApplyPercentage(
            int viewCount,
            int applyCount,
            int decimalPlaces,
            out double applyPercentage,
            out string errorMessage)
        {
            bool result = CalculateApplyPercentage(viewCount, applyCount, out applyPercentage, out errorMessage);

            if (result)
            {
                applyPercentage = Math.Round(applyPercentage, decimalPlaces);
            }

            return result;
        }

        /// <summary>
        /// Tính toán phần trăm tăng trưởng giữa hai khoảng thời gian
        /// </summary>
        /// <param name="previousCount">Số liệu của khoảng thời gian trước</param>
        /// <param name="currentCount">Số liệu của khoảng thời gian hiện tại</param>
        /// <returns>Phần trăm tăng trưởng</returns>
        public double CalculateGrowthPercentage(int previousCount, int currentCount)
        {
            if (previousCount == 0)
            {
                return currentCount > 0 ? 100 : 0;
            }

            return Math.Round(((double)(currentCount - previousCount) / previousCount) * 100, 2);
        }
    }
}