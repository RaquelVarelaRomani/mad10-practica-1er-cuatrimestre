using System;

namespace Es.Udc.DotNet.PracticaMAD.Model
{
    public partial class Category
    {
        public override bool Equals(object obj)
        {
            Category target = obj as Category;
            if (target == null)
            {
                return false;
            }

            return (this.categoryId == target.categoryId)
                && (this.categoryName == target.categoryName);
        }

        public override int GetHashCode()
        {
            return this.categoryId.GetHashCode();
        }

        public override string ToString()
        {
            return String.Format(
                "[ categoryId = {0} | categoryName = {1} ]",
                this.categoryId, this.categoryName);
        }
    }

    public partial class UserProfile
    {
        public override bool Equals(object obj)
        {
            UserProfile target = obj as UserProfile;
            if (target == null)
            {
                return false;
            }

            return (this.usrId == target.usrId)
                && (this.loginName == target.loginName)
                && (this.enPassword == target.enPassword)
                && (this.firstName == target.firstName)
                && (this.lastName == target.lastName)
                && (this.email == target.email)
                && (this.language == target.language)
                && (this.country == target.country);
        }

        public override int GetHashCode()
        {
            return this.usrId.GetHashCode();
        }

        public override string ToString()
        {
            return String.Format(
                "[ usrId = {0} | loginName = {1} | firstName = {2} | lastName = {3} | email = {4} | language = {5} | country = {6} ]",
                this.usrId, this.loginName, this.firstName, this.lastName, this.email, this.language, this.country);
        }
    }

    public partial class Product
    {
        public override bool Equals(object obj)
        {
            Product target = obj as Product;
            if (target == null)
            {
                return false;
            }

            return (this.productId == target.productId)
                && (this.productName == target.productName)
                && (this.creationDate == target.creationDate)
                && (this.description == target.description)
                && (this.image == target.image)
                && (this.categoryId == target.categoryId)
                && (this.sellerId == target.sellerId);
        }

        public override int GetHashCode()
        {
            return this.productId.GetHashCode();
        }

        public override string ToString()
        {
            return String.Format(
                "[ productId = {0} | productName = {1} | creationDate = {2} | description = {3} | image = {4} | categoryId = {5} | sellerId = {6} ]",
                this.productId, this.productName, this.creationDate, this.description, this.image, this.categoryId, this.sellerId);
        }
    }

    public partial class Comment
    {
        public override bool Equals(object obj)
        {
            Comment target = obj as Comment;
            if (target == null)
            {
                return false;
            }

            return (this.commentId == target.commentId)
                && (this.productId == target.productId)
                && (this.userId == target.userId)
                && (this.commentText == target.commentText)
                && (this.commentDate == target.commentDate)
                && (this.isInReview == target.isInReview);
        }

        public override int GetHashCode()
        {
            return this.commentId.GetHashCode();
        }

        public override string ToString()
        {
            return String.Format(
                "[ commentId = {0} | productId = {1} | userId = {2} | commentText = {3} | commentDate = {4} | isInReview = {5} ]",
                this.commentId, this.productId, this.userId, this.commentText, this.commentDate, this.isInReview);
        }
    }

    public partial class CommentReport
    {
        public override bool Equals(object obj)
        {
            CommentReport target = obj as CommentReport;
            if (target == null)
            {
                return false;
            }

            return (this.reportId == target.reportId)
                && (this.commentId == target.commentId)
                && (this.userId == target.userId)
                && (this.reason == target.reason)
                && (this.reportDate == target.reportDate);
        }

        public override int GetHashCode()
        {
            return this.reportId.GetHashCode();
        }

        public override string ToString()
        {
            return String.Format(
                "[ reportId = {0} | commentId = {1} | userId = {2} | reason = {3} | reportDate = {4} ]",
                this.reportId, this.commentId, this.userId, this.reason, this.reportDate);
        }
    }

    public partial class SellerRating
    {
        public override bool Equals(object obj)
        {
            SellerRating target = obj as SellerRating;
            if (target == null)
            {
                return false;
            }

            return (this.ratingId == target.ratingId)
                && (this.sellerId == target.sellerId)
                && (this.productId == target.productId)
                && (this.buyerUserId == target.buyerUserId)
                && (this.score == target.score)
                && (this.comment == target.comment)
                && (this.ratingDate == target.ratingDate);
        }

        public override int GetHashCode()
        {
            return this.ratingId.GetHashCode();
        }

        public override string ToString()
        {
            return String.Format(
                "[ ratingId = {0} | sellerId = {1} | productId = {2} | buyerUserId = {3} | score = {4} | comment = {5} | ratingDate = {6} ]",
                this.ratingId, this.sellerId, this.productId, this.buyerUserId, this.score, this.comment, this.ratingDate);
        }
    }

    public partial class Tag
    {
        public override bool Equals(object obj)
        {
            Tag target = obj as Tag;
            if (target == null)
            {
                return false;
            }

            return (this.tagId == target.tagId)
                && (this.tagName == target.tagName);
        }

        public override int GetHashCode()
        {
            return this.tagId.GetHashCode();
        }

        public override string ToString()
        {
            return String.Format(
                "[ tagId = {0} | tagName = {1} ]",
                this.tagId, this.tagName);
        }
    }
}

