### Manual Test Document for scAIentific Frontend

#### Introduction

This document provides a series of manual tests for the frontend of scAIentific, an AI-powered research front analyzer designed to assist researchers and students by simplifying the exploration of scientific articles.

#### Test Environment

- **Browser Compatibility**: Ensure that the tests are performed on multiple browsers (e.g., Chrome, Firefox, Safari) to validate cross-browser compatibility.
- **Test Data**: Prepare a word file that consists of scientific article abstracts to use as test data for uploading.

#### Test Cases

##### TC1.1: Home Page Load Test

**Objective**: Ensure the homepage loads correctly with all necessary components.
**Steps**:

1. Open a supported web browser.
2. Navigate to the scAIentific homepage URL "<https://cscloud7-45.lnu.se/>".
3. Observe if the homepage loads without errors.

**Expected Result**: The homepage should load with a welcome sentence, the navigation bar including login, register options, and the logo.

### TC2.1: User Registration

**Objective**: Test the functionality of new user registration.
**Steps**:

1. Navigate to the scAIentific homepage.
2. Click on the register option in the navbar.
3. Fill out the registration form with the following details:
   - Username: User-3
   - Password: User-3-user
   - Email: <user@user-3.com>
4. Submit the form.

**Expected Result**: The user should be registered successfully and redirected to a login page.

### TC2.2: User Registration with Registered Email

**Objective**: Test the registration process using an email address that is already registered.
**Steps**:

1. Navigate to the scAIentific homepage.
2. Click on the register option in the navbar.
3. Fill out the registration form with the following details:
   - Username: User-4
   - Password: User-4-user
   - Email: <user@user-3.com>
4. Submit the form.

**Expected Result**: Registration should be rejected, and an error message should be displayed indicating that the email address is already in use.

### TC2.3: User Registration with Registered Username

**Objective**: Test the registration process using a username that is already registered.
**Steps**:

1. Navigate to the scAIentific homepage.
2. Click on the register option in the navbar.
3. Fill out the registration form with the following details:
   - Username: User-3
   - Password: User-4-user
   - Email: <user@user-4.com>
4. Submit the form.

**Expected Result**: Registration should be rejected, and an error message should be displayed indicating that the username is already in use.


### TC3.1: User Login with Valid Credentials
**Objective**: Verify that a user can log in using valid credentials and is redirected appropriately.
**Steps**:
1. Navigate to the scAIentific homepage.
2. Click on the login option in the navbar.
3. Enter the following valid username and password:
   - Username: User-3
   - Password: User-3-user
4. Submit the login form.

**Expected Result**: The user should log in successfully and be redirected to the homepage, which displays a welcome sentence and a "Start" button.

### TC3.2: User Login with Invalid Credentials
**Objective**: Ensure that the login process handles invalid credentials correctly by preventing login and displaying an error message.
**Steps**:
1. Navigate to the scAIentific homepage.
2. Click on the login option in the navbar.
3. Enter the following credentials:
   - Username: User-3
   - Password: User-2-user
4. Submit the login form.

**Expected Result**: The login should fail. An error message should be displayed, indicating that the username or password is invalid.

##### TC4.1: User Logout

**Objective**: Ensure that the logout functionality works correctly.
**Steps**:
1. Navigate to the scAIentific homepage.
2. Complete TC3.1 to log in with valid credentials.
3. Click on the logout option in the navbar.
4. Confirm logout action if prompted.

**Expected Result**: The user should be logged out successfully and redirected to the homepage that has a welcome sentence .

### TC5.1.1: Create New Assistant with Over-sized File Upload
**Objective**: Verify the system's error handling when attempting to upload a file larger than the allowed size limit.
**Steps**:
1. Complete TC3.1 to log in with valid credentials.
2. Click on the 'Start' button on the homepage.
3. Enter "Gender and Social Networks" in the research area input field.
4. Attempt to upload a file larger than 512 MB.
5. Click 'Create Assistant'.

**Expected Result**: The assistant creation should be halted, and an error message should be displayed indicating that the file size exceeds the limit.

### TC5.1.2: Create New Assistant without Uploading a File
**Objective**: Confirm that the file upload is mandatory for creating an assistant.
**Steps**:
1. Follow TC3.1 to log in.
2. Click on the 'Start' button.
3. Input "Gender and Social Networks" in the research area field.
4. Do not upload any file.
5. Click 'Create Assistant'.

**Expected Result**: The assistant creation should be halted; no assistant is created, and an error message should display indicating that an article file is required.

### TC5.1.3: Create New Assistant with Unsupported File Type
**Objective**: Test the system's error handling when an unsupported file type is uploaded.
**Steps**:
1. Follow TC3.1 to log in.
2. Click on the 'Start' button.
3. Enter "Gender and Social Networks" in the research area field.
4. Attempt to upload a file that is not a Word document.
5. Click 'Create Assistant'.

**Expected Result**: The assistant creation should be halted; no assistant is created, and an error message should display indicating "Unsupported file type."

### TC5.2: Create New Assistant without Specifying Research Area
**Objective**: Verify that specifying a research area is mandatory for assistant creation.
**Steps**:
1. Follow TC3.1 to log in.
2. Click on the 'Start' button.
3. Leave the research area field empty.
4. Upload a correct file type within the size limit.
5. Click 'Create Assistant'.

**Expected Result**: The assistant creation should be halted; the file should not upload, and an error message should display indicating that the research area is required.

### TC5.4: Create New Assistant with Complete Information
**Objective**: Validate the complete assistant creation process including article upload and specifying the research area.
**Steps**:
1. Follow TC3.1 to log in.
2. Click on the 'Start' button.
3. Enter "Gender and Social Networks" in the research area field.
4. Browse and select the desired article file ensuring it's within the allowed size limit.
5. Click 'Create Assistant'.

**Expected Result**: The article should upload successfully, an assistant is created based on the specified research area, and the user is redirected to chat with the assistant page.

##### TC6.1: AI-Assistant Interaction

**Objective**:  Verify the functionality of interacting with the AI assistant using a valid query.
**Steps**:

1. Complete TC3.1 to log in with valid credentials.
2. click on the start button, you will be redirected to the chat with the assistant page.
3. Enter a query related to the uploaded file.
4. click on the sent button  .

**Expected Result**: The AI assistant should provide a coherent and contextually appropriate response.

##### TC6.2: AI-Assistant Interaction with Empty Query

**Objective**: Verify that the system handles empty queries appropriately.
**Steps**:

1. Complete TC3.1 to log in with valid credentials.
2. Navigate to the AI chat interface.
3. Leave the input field empty.
4. Click on the 'Send' button.

**Expected Result**: The user is unable to send an empty message.

##### TC7.1: Retrieve Chat History

**Objective**:Verify that the chat history is preserved and retrievable after logging out and back in.
**Steps**:

1. Complete TC3.1 to log in with valid credentials.
2. Complete TC6.1 to interact with the AI assistant and generate chat history.
3. Log out of the application.
4. Complete TC3.1 again to log back in with the same credentials.
5. Navigate to the AI chat interface.

**Expected Result**:The user should see the previous chat history including the last conversation when landing on the chat with the assistant page.

##### TC8.2: Delete the assistant

**Objective**: Test Deleting the assistant.
**Steps**:

1. Complete TC3.1 to log in with valid credentials
2. Click on delete assistant.

**Expected Result**:  The assistant is deleted and the user is redirected to the home page.

#### Conclusion

This manual test document outlines the key functionalities of the scAIentific frontend that need to be rigorously tested to ensure a quality experience for its users. Each test case is designed to verify that every component of the application performs as expected under various conditions.
