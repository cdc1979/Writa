using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using FluentValidation.Validators;

namespace Writa.Models.Validation
{
    public class WritaPostValidator : AbstractValidator<WritaPost>
    {
        IDataHelper _d;

        public WritaPostValidator(IDataHelper d, WritaPost current)
        {
            _d = d;
            RuleFor(w => w.PostTitle).NotEmpty().WithMessage("Please specify a post title");
            RuleFor(w => w.PostSlug).NotEmpty().WithMessage("Please specify a post url");
            RuleFor(w => w.PostId).NotEmpty();

        }

    }

    public static class ValidationHelper
    {
        public static ValidationResult ValidateWritaPost(WritaPost p, IDataHelper d)
        {
            WritaPostValidator validator = new WritaPostValidator(d, p);
            ValidationResult results = validator.Validate(p);
            return results;
        }
    }
}
