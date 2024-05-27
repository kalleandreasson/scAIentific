# scAIentific: AI-powered Research Front Analyzer

## Overview

scAIentific is an innovative application tailored specifically for researchers and students deeply involved in their specific fields of study. This advanced AI-powered tool specializes in providing expert assistance by analyzing a vast array of scientific articles to identify key trends, themes, and findings that align precisely with the user’s current research focus. scAIentific is designed to streamline the research process, making the exploration of cutting-edge research more efficient and accessible. It saves valuable time and resources, enhancing productivity and insight. This project is the result of a collaborative effort by students as part of a university group initiative, showcasing the practical application of AI in academic research.

## Developers

This project was developed by a group of students:

- **[Shirin Meirkhan](https://github.com/Shirin20)**
- **[kalle Andreasson](https://github.com/kalleandreasson)**

### Project Origin

The need for scAIentific arose from the challenges faced by researchers in summarizing extensive previous works, a task particularly daunting when writing dissertations or scientific papers. An example project examining gender differences in networking during college years highlighted the labor-intensive process of manually identifying relevant research trends from a pool of 250 scientific articles.

### Purpose

scAIentific's goal is to explore how AI can facilitate and streamline the process of research front analysis, potentially uncovering insights that manual methods might miss.

### Project Goals

- Develop an app that gives the user the ability to create an Ai assistant, that is expert in a research area of the user choice, that will help the user to find and describe the current research front.
- Provide a user-friendly frontend for uploading research articles and receiving the needed results.

## Features

- **Article Upload**: Users can upload files (.docx) up to 512 MB directly into the AI assistant.
- **Custom AI-Assistant Creation**: Allows users to create a personalized AI assistant. This specialized assistant is proficient in a research field specified by the user.
- **Research Discussion with the AI-Assistant**: Offers the capability to converse with the assistant and discuss the file's content, assisting in the identification of the research front.
- **The AI-Assistant chat history**: The user will get the chat history sent to their mail.

## Technical Stack

- **Frontend**: Blazor
- **Backend**: C#, .NET
- **Ai Model**: Open ai chat GPT 4.

## Getting Started

### Prerequisites

- .NET core 8.
- An IDE such as Visual Studio or Visual Studio Code
- Access to the provided AI model or equivalent
- Create a mail for the application to enable that chat history sending feature.
- Set mongoBD Atlas database connectionstring.

### Setup

1. Clone the repository to your local machine.
2. Ensure all required software and dependencies are installed.
3. Navigate to the project directory and restore the required packages:

```shell
dotnet restore
```

4. Start the backend server:

```shell
dotnet run --project Path/To/BackendProject
```

5. Start the frontend application:

```shell
cd Path/To/FrontendProject
dotnet run
```

## Usage

- Navigate to the frontend application through your web browser.
- Register a new user, login and create your Ai research assistant and start interacting with it.  

## Project design
![assistant-page](AppOverview/assistant-page.png)  
![uploadFile-page](AppOverview/uploadFile-page.png) 
![Home-page](AppOverview/home-page.png) 

## License
MIT