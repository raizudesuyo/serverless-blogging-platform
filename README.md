# Serverless Blogging Platform

GOAL: To create a blogging platform to be fully serverless, all code available in github

## Stories

- As a User I must be able to login using my google account
- As a User I must be able to login using my facebook account
- As a User I must be able to login using my x account
- As a User I must be able to post blogs
- As a User I must be able to upload image in my blogs
- As a User I must be able to comment on other people's blogs and my blogs
- As a User I must be able to comment images on other people's blogs
- As a User I must be able to see my previous works
- As a User I must be able to view other users blogs
- As a User or a Guest, when I visit the page, I should get recommendations on what to read
- As a User or a Guest, I should get the hottest recommendations, and recommendations based on undiscovered writers
- As a User I should be able to put tags on my blogposts

## Technical

- Uses AWS Cognito for user logins
- ReactJS + .NET Serverless backend, wherein ReactJS is uploaded in S3
- Database is in DynamoDB
- Uses AWS DevOps stuff )Will be done when I'm a devops pro)
- Frontend will use tailwind, will be very basic in it's design

## Data Model

```mermaid
---
title: Using DynamoDB
---
erDiagram
    POST ||--o{ POST_COMMENT: "has many"
    POST ||--o{ POST_ASSET: "has uploaded"
    POST ||--o{ VIEW: "When user Views"
    POST {
        uuid postId PK
        string title
        string content
        string authorId FK
    }
    POST_COMMENT {
        uuid postCommentId PK
        uuid postId FK
        uuid authorId FK
        string content
        uuid parentCommentId FK
    }
    POST_ASSET {
        uuid assetId PK
        string name
        url assetUrl
    }
    VIEW {
        uuid viewId PK
        uuid userId FK
        uuid postId PK
    }
```

```mermaid
classDiagram
namespace Controllers {
    class PostController {
        +getPosts(pagination page)
        +getPost(uuid postId)
        +getUserPosts(uuid userId, pagination page)
        +getPostsByTag(string tagName, pagination page)
        +createPost(PostDTO args)
        +updatePost(uuid postId, UpdatePostDTO args)
        +createComment(uuid postId, CreateCommentDTO args)
    }
    class AssetController {
        +uploadAsset()
        +getAssetUrl(uuid assetUrl)
    }
    class CommentController {
        +createComment(uuid postId, CreateCommentDTO dto)
        +getComments(uuid postId, pagination page)
    }
}
```