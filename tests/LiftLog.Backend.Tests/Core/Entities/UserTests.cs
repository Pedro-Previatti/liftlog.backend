using LiftLog.Backend.Core.Entities;
using LiftLog.Backend.Core.Enums;

namespace LiftLog.Backend.Tests.Core.Entities;

public class UserTests
{
    private readonly Guid _validGuid = Guid.NewGuid();
    private readonly Guid _invalidGuid = Guid.Empty;
    private readonly string _emptyString = string.Empty;
    private const UserType Type = UserType.Student;
    private const UserType InvalidType = (UserType)int.MaxValue;
    private const Gender BaseGender = Gender.Undefined;
    private const Gender MaleGender = Gender.Male;
    private const Gender InvalidGender = (Gender)int.MaxValue;
    private const HeightUnit ValidHeightUnit = HeightUnit.Centimeters;
    private const HeightUnit InvalidHeightUnit = (HeightUnit)int.MaxValue;
    private const WeightUnit ValidWeightUnit = WeightUnit.Kilograms;
    private const WeightUnit InvalidWeightUnit = (WeightUnit)int.MaxValue;
    private const string SmallName = "fn";
    private const string Name = "John";
    private const string HugeName = "This Name Is Way Too Big To Be Real";
    private const string InvalidNameCharacters = "N0t 4 Val!d N4m3 #";
    private const string Cpf = "435.417.050-78";
    private const string InvalidCpf = "43541705078";
    private const string InvalidCpfNumbers = "435.417.050-79";
    private const string PhoneNumber = "+55 (62) 94002-8922";
    private const string InvalidPhoneNumber = "5562940028922";
    private const string Email = "press@google.com";
    private const string InvalidEmail = "pressgoogle.com";
    private const string ValidPassword =
        "$2a$12$SfAvCxMBP6uB7A7fsbPOtO3eo910DId.iOSn25RbEyXfMFuDwKpKC";
    private const string InvalidPassword = "12345JohnDoe!@#$";
    private const float Float = float.MaxValue;
    private const float NegativeFloat = float.MinValue;
    private const float NanFloat = float.NaN;
    private const float InfinityFloat = float.PositiveInfinity;
    private const float NegativeInfinityFloat = float.NegativeInfinity;

    [Fact(DisplayName = "User => Create() Returns Success")]
    [Trait("Category", "Entity")]
    public void User_Create_ReturnsSuccess()
    {
        var userOne = User.Create(
            Type,
            _validGuid,
            Name,
            Name,
            Cpf,
            PhoneNumber,
            Email,
            ValidPassword
        );
        var userTwo = User.Create(Type, null, Name, Name, Cpf, PhoneNumber, Email, ValidPassword);

        Assert.NotNull(userOne);
        Assert.True(userOne.Valid);
        Assert.Equal(Type, userOne.Type);
        Assert.Equal(_validGuid, userOne.TeacherId);
        Assert.Equal(Name, userOne.FirstName);
        Assert.Equal(Name, userOne.LastName);
        Assert.Equal(Cpf, userOne.Cpf);
        Assert.Equal(PhoneNumber, userOne.PhoneNumber);
        Assert.Equal(Email, userOne.Email);
        Assert.Equal(ValidPassword, userOne.Password);
        Assert.Equal(BaseGender, userOne.Gender);
        Assert.Equal($"{Name} {Name} {Cpf} {PhoneNumber} {Email}", userOne.SearchText);

        Assert.NotNull(userTwo);
        Assert.True(userTwo.Valid);
        Assert.Equal(Type, userOne.Type);
        Assert.Null(userTwo.TeacherId);
        Assert.Equal(Name, userTwo.FirstName);
        Assert.Equal(Name, userTwo.LastName);
        Assert.Equal(Cpf, userTwo.Cpf);
        Assert.Equal(PhoneNumber, userTwo.PhoneNumber);
        Assert.Equal(Email, userTwo.Email);
        Assert.Equal(ValidPassword, userTwo.Password);
        Assert.Equal(BaseGender, userTwo.Gender);
        Assert.Equal($"{Name} {Name} {Cpf} {PhoneNumber} {Email}", userTwo.SearchText);
    }

    [Fact(DisplayName = "User => Create() Returns Invalid")]
    [Trait("Category", "Entity")]
    public void User_Create_ReturnsInvalid()
    {
        var userOne = User.Create(
            InvalidType,
            _invalidGuid,
            _emptyString,
            _emptyString,
            _emptyString,
            _emptyString,
            _emptyString,
            _emptyString
        );
        var userTwo = User.Create(
            Type,
            null,
            SmallName,
            SmallName,
            InvalidCpf,
            InvalidPhoneNumber,
            InvalidEmail,
            InvalidPassword
        );
        var userThree = User.Create(
            Type,
            null,
            HugeName,
            HugeName,
            InvalidCpfNumbers,
            PhoneNumber,
            Email,
            ValidPassword
        );
        var userFour = User.Create(
            Type,
            null,
            InvalidNameCharacters,
            InvalidNameCharacters,
            Cpf,
            PhoneNumber,
            Email,
            ValidPassword
        );

        Assert.True(userOne.Invalid);
        CollectionAssert.IsNotEmpty(userOne.GetNotifications());
        var userOneNotifications = userOne.GetNotifications();
        Assert.Contains(userOneNotifications, x => x.Message == "Type must be a valid enum value.");
        Assert.Contains(
            userOneNotifications,
            x => x.Message == "TeacherId must be a valid Guid when provided."
        );
        Assert.Contains(userOneNotifications, x => x.Message == "FirstName is required.");
        Assert.Contains(userOneNotifications, x => x.Message == "LastName is required.");
        Assert.Contains(userOneNotifications, x => x.Message == "CPF is required.");
        Assert.Contains(userOneNotifications, x => x.Message == "PhoneNumber is required.");
        Assert.Contains(userOneNotifications, x => x.Message == "Email is required.");
        Assert.Contains(userOneNotifications, x => x.Message == "Password is required.");

        Assert.True(userTwo.Invalid);
        CollectionAssert.IsNotEmpty(userTwo.GetNotifications());
        var userTwoNotifications = userTwo.GetNotifications();
        Assert.Contains(
            userTwoNotifications,
            x => x.Message == "FirstName must have more than 2 characters."
        );
        Assert.Contains(
            userTwoNotifications,
            x => x.Message == "LastName must have more than 2 characters."
        );
        Assert.Contains(
            userTwoNotifications,
            x => x.Message == "CPF must be in the format XXX.XXX.XXX-XX."
        );
        Assert.Contains(
            userTwoNotifications,
            x => x.Message == "PhoneNumber must be in the format: +CC (AA) 99999-9999."
        );
        Assert.Contains(
            userTwoNotifications,
            x => x.Message == "Email is not a valid email address."
        );

        Assert.True(userThree.Invalid);
        CollectionAssert.IsNotEmpty(userThree.GetNotifications());
        var userThreeNotifications = userThree.GetNotifications();
        Assert.Contains(
            userThreeNotifications,
            x => x.Message == "FirstName must be up to 20 characters."
        );
        Assert.Contains(
            userThreeNotifications,
            x => x.Message == "LastName must be up to 20 characters."
        );
        Assert.Contains(userThreeNotifications, x => x.Message == "CPF must be valid.");

        Assert.True(userFour.Invalid);
        CollectionAssert.IsNotEmpty(userFour.GetNotifications());
        var userFourNotifications = userFour.GetNotifications();
        Assert.Contains(
            userFourNotifications,
            x => x.Message == "FirstName contains invalid characters."
        );
        Assert.Contains(
            userFourNotifications,
            x => x.Message == "LastName contains invalid characters."
        );
    }

    [Fact(DisplayName = "User => Update() Returns Success")]
    [Trait("Category", "Entity")]
    public void User_Update_ReturnsSuccess()
    {
        var user = User.Create(Type, null, Name, Name, Cpf, PhoneNumber, Email, ValidPassword);
        user.Update(
            null,
            MaleGender,
            ValidHeightUnit,
            ValidWeightUnit,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            Float,
            Float
        );

        Assert.NotNull(user);
        Assert.True(user.Valid);
        Assert.Equal(Type, user.Type);
        Assert.Null(user.TeacherId);
        Assert.Equal(Name, user.FirstName);
        Assert.Equal(Name, user.LastName);
        Assert.Equal(Cpf, user.Cpf);
        Assert.Equal(PhoneNumber, user.PhoneNumber);
        Assert.Equal(Email, user.Email);
        Assert.Equal(ValidPassword, user.Password);
        Assert.Equal(MaleGender, user.Gender);
        Assert.Equal(ValidHeightUnit, user.HeightUnit);
        Assert.Equal(ValidWeightUnit, user.WeightUnit);
        Assert.Equal(Float, user.Weight);
        Assert.Equal(Float, user.Weight);
        Assert.Equal($"{Name} {Name} {Cpf} {PhoneNumber} {Email}", user.SearchText);
    }

    [Theory(DisplayName = "User => Update() Returns Invalid")]
    [Trait("Category", "Entity")]
    [InlineData(NegativeFloat)]
    [InlineData(NanFloat)]
    [InlineData(InfinityFloat)]
    [InlineData(NegativeInfinityFloat)]
    public void User_Update_ReturnsInvalid(float invalidFloat)
    {
        var userOne = User.Create(Type, null, Name, Name, Cpf, PhoneNumber, Email, ValidPassword);
        userOne.Update(
            InvalidType,
            InvalidGender,
            InvalidHeightUnit,
            InvalidWeightUnit,
            _invalidGuid,
            InvalidNameCharacters,
            InvalidNameCharacters,
            InvalidCpf,
            InvalidPhoneNumber,
            InvalidEmail,
            InvalidPassword,
            invalidFloat,
            invalidFloat
        );

        Assert.True(userOne.Invalid);
        CollectionAssert.IsNotEmpty(userOne.GetNotifications());
        var userOneNotifications = userOne.GetNotifications();
        Assert.Contains(userOneNotifications, x => x.Message == "Type must be a valid enum value.");
        Assert.Contains(
            userOneNotifications,
            x => x.Message == "Gender must be a valid enum value when provided."
        );
        Assert.Contains(
            userOneNotifications,
            x => x.Message == "HeightUnit must be a valid enum value when provided."
        );
        Assert.Contains(
            userOneNotifications,
            x => x.Message == "WeightUnit must be a valid enum value when provided."
        );
        Assert.Contains(
            userOneNotifications,
            x => x.Message == "TeacherId must be a valid Guid when provided."
        );
        Assert.Contains(
            userOneNotifications,
            x => x.Message == "FirstName contains invalid characters."
        );
        Assert.Contains(
            userOneNotifications,
            x => x.Message == "LastName contains invalid characters."
        );
        Assert.Contains(
            userOneNotifications,
            x => x.Message == "CPF must be in the format XXX.XXX.XXX-XX."
        );
        Assert.Contains(
            userOneNotifications,
            x => x.Message == "PhoneNumber must be in the format: +CC (AA) 99999-9999."
        );
        Assert.Contains(
            userOneNotifications,
            x => x.Message == "Email is not a valid email address."
        );
        Assert.Contains(userOneNotifications, x => x.Message == "Password must be stored in hash.");
        Assert.Contains(
            userOneNotifications,
            x => x.Message == "Height must be a valid positive number when provided."
        );
        Assert.Contains(
            userOneNotifications,
            x => x.Message == "Weight must be a valid positive number when provided."
        );
    }
}
