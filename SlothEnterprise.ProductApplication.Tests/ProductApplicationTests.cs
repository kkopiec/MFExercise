using FluentAssertions;
using Moq;
using SlothEnterprise.External;
using SlothEnterprise.External.V1;
using SlothEnterprise.ProductApplication.Applications;
using SlothEnterprise.ProductApplication.Products;
using System;
using Xunit;

namespace SlothEnterprise.ProductApplication.Tests
{
    public class ProductApplicationTests
    {
        private readonly IProductApplicationService _sut;
        private readonly Mock<IConfidentialInvoiceService> _confidentialInvoiceServiceMock = new Mock<IConfidentialInvoiceService>();
        private readonly Mock<ISelectInvoiceService> _selectInvoiceServiceMock = new Mock<ISelectInvoiceService>();
        private readonly Mock<IBusinessLoansService> _businessLoansServiceMock = new Mock<IBusinessLoansService>();
        private readonly Mock<ISellerApplication> _sellerApplicationMock = new Mock<ISellerApplication>();
        private readonly Mock<IApplicationResult> _result = new Mock<IApplicationResult>();
        private ISellerApplication sellerApplication;

        public class NotValidClass : IProduct
        {
            public int Id => 1;
        }
        public ProductApplicationTests()
        {
            _sut = new ProductApplicationService(_selectInvoiceServiceMock.Object, _confidentialInvoiceServiceMock.Object, _businessLoansServiceMock.Object);
            sellerApplication = _sellerApplicationMock.Object;
            
            _confidentialInvoiceServiceMock.Setup(m => m.SubmitApplicationFor(It.IsAny<CompanyDataRequest>(), It.IsAny<decimal>(), It.IsAny<decimal>(), It.IsAny<decimal>())).Returns(_result.Object);
            _businessLoansServiceMock.Setup(m => m.SubmitApplicationFor(It.IsAny<CompanyDataRequest>(), It.IsAny<LoansRequest>())).Returns(_result.Object);
        }
        private IProduct GetProductData<T>() where T:IProduct
        {
            if (typeof(T) == typeof(ConfidentialInvoiceDiscount))
            {
                return new ConfidentialInvoiceDiscount()
                {
                    AdvancePercentage = 2.0m,
                    Id = 1,
                    TotalLedgerNetworth = 4m,
                    VatRate = 18m

                };
            }
            if (typeof(T) == typeof(SelectiveInvoiceDiscount))
            {
                return new SelectiveInvoiceDiscount()
                {
                    AdvancePercentage = 18m,
                    Id = 1,
                    InvoiceAmount = 20m

                };
            }
            if (typeof(T) == typeof(BusinessLoans))
            {
                return new BusinessLoans()
                {
                    Id = 1,
                    InterestRatePerAnnum = 7.5m,
                    LoanAmount = 2000m
                };
            }
            if (typeof(T) == typeof(NotValidClass))
            {
                return new NotValidClass();
            }
            throw new ArgumentException("Invalid object type");
        }
        private SellerCompanyData GetSellerCompanyMockupData()
        {
                return new SellerCompanyData()
                {
                    DirectorName = "DirectorName",
                    Founded = new System.DateTime(2020, 12, 7),
                    Name = "Name",
                    Number = 10
                };
        
        }
        private void setupResult(bool success, int? applicationId)
        {
            _result.SetupProperty(p => p.ApplicationId, applicationId);
            _result.SetupProperty(p => p.Success, success);
        }
        private void setupSelectInvoiceServiceResult(int result)
        {
            _selectInvoiceServiceMock.Setup(m => m.SubmitApplicationFor(It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<decimal>())).Returns(result);
        }
        private void setupSellerApplication<T>() where T:IProduct
        {
            _sellerApplicationMock.SetupProperty(p => p.Product, GetProductData<T>());
            _sellerApplicationMock.SetupProperty(p => p.CompanyData, GetSellerCompanyMockupData());
        }


        [Fact]
        public void ProductApplicationService_SubmitApplicationFor_WhenCalledWithSelectiveInvoiceDiscount_ShouldReturnOne()
        {
            // Arrange
            setupSellerApplication<SelectiveInvoiceDiscount>();
            setupSelectInvoiceServiceResult(1);
            // Act
            var result = _sut.SubmitApplicationFor(sellerApplication);
            // Assert
            result.Should().Be(1);
        }
        [Fact]
        public void ProductApplicationService_SubmitApplicationFor_WhenCalledWithSelectiveInvoiceDiscount_ShouldReturnNegOne()
        {
            // Arrange
            setupSellerApplication<SelectiveInvoiceDiscount>();
            setupSelectInvoiceServiceResult(-1);
            // Act
            var result = _sut.SubmitApplicationFor(sellerApplication);
            // Assert
            result.Should().Be(-1);
        }
        [Fact]
        public void ProductApplicationService_SubmitApplicationFor_WhenConfidentialInvoiceDiscount_ShouldReturnTwo()
        {
            // Arrange
            setupSellerApplication<ConfidentialInvoiceDiscount>();
            setupResult(true, 2);
            // Act
            var result = _sut.SubmitApplicationFor(sellerApplication);
            // Assert
            result.Should().Be(2);
        }
        [Fact]
        public void ProductApplicationService_SubmitApplicationFor_WhenConfidentialInvoiceDiscount_ShouldReturnNegOne()
        {
            // Arrange
            setupSellerApplication<ConfidentialInvoiceDiscount>();
            setupResult(false, null);
            // Act
            var result = _sut.SubmitApplicationFor(sellerApplication);
            // Assert
            result.Should().Be(-1);
        }
        [Fact]
        public void ProductApplicationService_SubmitApplicationFor_WhenConfidentialInvoiceDiscount_NoApplicationID_ShouldReturnNegOne()
        {
            // Arrange
            setupSellerApplication<ConfidentialInvoiceDiscount>();
            setupResult(true, null);
            // Act
            var result = _sut.SubmitApplicationFor(sellerApplication);
            // Assert
            result.Should().Be(-1);
        }
        [Fact]
        public void ProductApplicationService_SubmitApplicationFor_WhenBusinessLoans_ShouldReturnTwo()
        {
            // Arrange
            setupSellerApplication<BusinessLoans>();
            setupResult(true, 2);
            // Act
            var result = _sut.SubmitApplicationFor(sellerApplication);
            // Assert
            result.Should().Be(2);
        }
        [Fact]
        public void ProductApplicationService_SubmitApplicationFor_WhenBusinessLoans_ShouldReturnNegOne()
        {
            // Arrange
            setupSellerApplication<BusinessLoans>();
            setupResult(false, null);
            // Act
            var result = _sut.SubmitApplicationFor(sellerApplication);
            // Assert
            result.Should().Be(-1);
        }
        [Fact]
        public void ProductApplicationService_SubmitApplicationFor_WhenBusinessLoans_NoApplicationID_ShouldReturnNegOne()
        {
            // Arrange
            setupSellerApplication<BusinessLoans>();
            setupResult(true, null);
            // Act
            var result = _sut.SubmitApplicationFor(sellerApplication);
            // Assert
            result.Should().Be(-1);
        }
        [Fact]
        public void ProductApplicationService_SubmitApplicationFor_WhenNotValidClass_ShouldThrowException()
        {
            // Arrange
            setupSellerApplication<NotValidClass>();
            
            // Act // Assert
            var result = Assert.Throws<InvalidOperationException>(()=>_sut.SubmitApplicationFor(sellerApplication));

        }
    }
}