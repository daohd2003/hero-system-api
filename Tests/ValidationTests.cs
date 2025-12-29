using BusinessObject.DTOs;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace Tests
{
    public class ValidationTests
    {
        // Helper method để validate object
        private List<ValidationResult> ValidateModel(object model)
        {
            var results = new List<ValidationResult>();
            var context = new ValidationContext(model);
            Validator.TryValidateObject(model, context, results, true);
            return results;
        }

        #region CreateHeroDto Validation Tests

        [Fact]
        public void CreateHeroDto_ValidData_NoValidationErrors()
        {
            var dto = new CreateHeroDto
            {
                Name = "Iron Man",
                SuperPower = "Money",
                Level = 50
            };

            var results = ValidateModel(dto);

            Assert.Empty(results);
        }

        [Fact]
        public void CreateHeroDto_EmptyName_ReturnsValidationError()
        {
            var dto = new CreateHeroDto
            {
                Name = "",
                SuperPower = "Power",
                Level = 50
            };

            var results = ValidateModel(dto);

            Assert.Contains(results, r => r.ErrorMessage == "Tên không được để trống");
        }

        [Fact]
        public void CreateHeroDto_NullName_ReturnsValidationError()
        {
            var dto = new CreateHeroDto
            {
                Name = null!,
                SuperPower = "Power",
                Level = 50
            };

            var results = ValidateModel(dto);

            Assert.Contains(results, r => r.ErrorMessage == "Tên không được để trống");
        }

        [Fact]
        public void CreateHeroDto_NameTooLong_ReturnsValidationError()
        {
            var dto = new CreateHeroDto
            {
                Name = new string('A', 51), // 51 ký tự
                SuperPower = "Power",
                Level = 50
            };

            var results = ValidateModel(dto);

            Assert.Contains(results, r => r.ErrorMessage!.Contains("Tên quá dài"));
        }

        [Fact]
        public void CreateHeroDto_LevelBelowRange_ReturnsValidationError()
        {
            var dto = new CreateHeroDto
            {
                Name = "Hero",
                SuperPower = "Power",
                Level = -1
            };

            var results = ValidateModel(dto);

            Assert.Contains(results, r => r.ErrorMessage!.Contains("Level"));
        }

        [Fact]
        public void CreateHeroDto_LevelAboveRange_ReturnsValidationError()
        {
            var dto = new CreateHeroDto
            {
                Name = "Hero",
                SuperPower = "Power",
                Level = 101
            };

            var results = ValidateModel(dto);

            Assert.Contains(results, r => r.ErrorMessage!.Contains("Level"));
        }

        [Fact]
        public void CreateHeroDto_NullSuperPower_ReturnsValidationError()
        {
            var dto = new CreateHeroDto
            {
                Name = "Hero",
                SuperPower = null!,
                Level = 50
            };

            var results = ValidateModel(dto);

            Assert.NotEmpty(results);
        }

        #endregion

        #region UpdateHeroDto Validation Tests

        [Fact]
        public void UpdateHeroDto_ValidData_NoValidationErrors()
        {
            var dto = new UpdateHeroDto
            {
                Name = "Spider Man",
                SuperPower = "Web",
                Level = 80,
                FactionId = Guid.NewGuid()
            };

            var results = ValidateModel(dto);

            Assert.Empty(results);
        }

        [Fact]
        public void UpdateHeroDto_EmptyName_ReturnsValidationError()
        {
            var dto = new UpdateHeroDto
            {
                Name = "",
                SuperPower = "Power",
                Level = 50
            };

            var results = ValidateModel(dto);

            Assert.Contains(results, r => r.ErrorMessage == "Tên không được để trống");
        }

        [Fact]
        public void UpdateHeroDto_NameTooLong_ReturnsValidationError()
        {
            var dto = new UpdateHeroDto
            {
                Name = new string('B', 51),
                SuperPower = "Power",
                Level = 50
            };

            var results = ValidateModel(dto);

            Assert.Contains(results, r => r.ErrorMessage!.Contains("Tên quá dài"));
        }

        [Fact]
        public void UpdateHeroDto_LevelOutOfRange_ReturnsValidationError()
        {
            var dto = new UpdateHeroDto
            {
                Name = "Hero",
                SuperPower = "Power",
                Level = 150
            };

            var results = ValidateModel(dto);

            Assert.Contains(results, r => r.ErrorMessage!.Contains("Level"));
        }

        [Fact]
        public void UpdateHeroDto_NullFactionId_IsValid()
        {
            var dto = new UpdateHeroDto
            {
                Name = "Hero",
                SuperPower = "Power",
                Level = 50,
                FactionId = null
            };

            var results = ValidateModel(dto);

            Assert.Empty(results);
        }

        #endregion

        #region CreateMissionDto Validation Tests

        [Fact]
        public void CreateMissionDto_ValidData_NoValidationErrors()
        {
            var dto = new MissionDtos.CreateMissionDto
            {
                Title = "Save the World",
                DifficultyLevel = 10
            };

            var results = ValidateModel(dto);

            Assert.Empty(results);
        }

        [Fact]
        public void CreateMissionDto_NullTitle_ReturnsValidationError()
        {
            var dto = new MissionDtos.CreateMissionDto
            {
                Title = null!,
                DifficultyLevel = 5
            };

            var results = ValidateModel(dto);

            Assert.NotEmpty(results);
        }

        [Fact]
        public void CreateMissionDto_EmptyTitle_ReturnsValidationError()
        {
            var dto = new MissionDtos.CreateMissionDto
            {
                Title = "",
                DifficultyLevel = 5
            };

            var results = ValidateModel(dto);

            Assert.NotEmpty(results);
        }

        #endregion

        #region CreateFactionDto Validation Tests

        [Fact]
        public void CreateFactionDto_ValidData_NoValidationErrors()
        {
            var dto = new FactionDtos.CreateFactionDto
            {
                Name = "Avengers",
                Description = "Earth's Mightiest Heroes"
            };

            var results = ValidateModel(dto);

            Assert.Empty(results);
        }

        [Fact]
        public void CreateFactionDto_NullName_ReturnsValidationError()
        {
            var dto = new FactionDtos.CreateFactionDto
            {
                Name = null!,
                Description = "Some description"
            };

            var results = ValidateModel(dto);

            Assert.NotEmpty(results);
        }

        [Fact]
        public void CreateFactionDto_NullDescription_IsValid()
        {
            var dto = new FactionDtos.CreateFactionDto
            {
                Name = "X-Men",
                Description = null!
            };

            var results = ValidateModel(dto);

            Assert.Empty(results);
        }

        #endregion
    }
}
