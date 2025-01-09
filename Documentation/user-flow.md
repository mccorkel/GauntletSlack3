# ChatGenius User Flow

This document maps out the complete journey of users and data through the ChatGenius application.

## User Registration and Login

1.  **User Registration:**
    *   The user navigates to the ChatGenius registration page.
    *   The user enters their username, email address, and password.
    *   The client-side validates the input format.
    *   The client sends the registration data to the server.
    *   The server validates the data and checks for existing users with the same username or email.
    *   The server hashes the password using a secure hashing algorithm.
    *   The server stores the user data in the database.
    *   The server sends an email verification link to the user.
    *   The user clicks the verification link to activate their account.
2.  **User Login:**
    *   The user navigates to the ChatGenius login page.
    *   The user enters their email/username and password.
    *   The client-side validates the input format.
    *   The client sends the login credentials to the server.
    *   The server retrieves the user's hashed password from the database.
    *   The server verifies the provided password against the stored hash.
    *   If successful, the server generates an authentication token (e.g., JWT).
    *   The server sends the token to the client.
    *   The client stores the token securely (e.g., in local storage).
    *   The client uses the token to authenticate subsequent requests to the server.

## Channel and DM Interactions

1.  **Joining a Channel:**
    *   The user views a list of available channels.
    *   The user selects a channel to join.
    *   The client sends a request to the server to join the channel.
    *   The server adds the user to the channel's member list.
    *   The server sends a notification to other channel members about the new user.
    *   The client updates the UI to reflect the user's presence in the channel.
2.  **Sending a Message:**
    *   The user types a message in the channel or DM input field.
    *   The user can add emojis or attachments to the message.
    *   The user clicks the "Send" button.
    *   The client sends the message data to the server.
    *   The server validates the message content.
    *   The server stores the message in the database.
    *   The server publishes the message to the relevant channel/DM topic using Azure Pub/Sub.
    *   Clients subscribed to the topic receive the message in real-time.
    *   Clients update their UI to display the new message.
3.  **Creating a Thread:**
    *   The user replies to a specific message to create a thread.
    *   The client sends the thread reply to the server.
    *   The server stores the thread reply in the database, linking it to the parent message.
    *   The server publishes the thread reply to the relevant channel/DM topic.
    *   Clients subscribed to the topic receive the thread reply and update their UI.
4.  **Sharing a File:**
    *   The user selects a file to upload.
    *   The client sends the file to the server.
    *   The server validates the file type and size.
    *   The server stores the file securely (e.g., in Azure Blob Storage).
    *   The server stores the file metadata in the database.
    *   The server sends a notification to other users in the channel/DM about the new file.

## Real-time Updates

1.  **Presence Updates:**
    *   The client periodically sends presence updates (online, away, offline) to the server.
    *   The server publishes presence updates to a dedicated presence topic.
    *   Clients subscribed to the presence topic receive updates and update their UI to reflect user status.
2.  **Typing Indicators:**
    *   When a user starts typing, the client sends a "typing" notification to the server.
    *   The server publishes the "typing" notification to the relevant channel/DM topic.
    *   Other clients in the channel/DM receive the notification and display a typing indicator.
    *   When the user stops typing, the client sends a "stopped typing" notification, and the server updates the presence accordingly.

## Data Flow

*   User data (registration, login, profile) is stored in the `Users` table in the database.
*   Channel and DM data is stored in the `Channels` and `DirectMessages` tables.
*   Message data is stored in the `Messages` table, linked to the relevant channel/DM.
*   File metadata is stored in the `Files` table, with the actual files stored in Azure Blob Storage.
*   Thread data is stored in the `Threads` table, linked to the parent message.
*   Real-time updates (presence, typing indicators) are handled through Azure Pub/Sub topics.

This `user-flow.md` document provides a comprehensive overview of the user journey and data flow within ChatGenius. It serves as a roadmap for developers and stakeholders to understand how users will interact with the application and how data will be managed.