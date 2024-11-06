#include <iostream>
#include <WinSock2.h>
#include <WS2tcpip.h>
#include <vector>
#include <string>
#include <fstream>

#pragma comment(lib, "Ws2_32.lib")

using namespace std;

void processFile(const string& fileName, int insertions, string& result) {
    ifstream inFile(fileName);
    if (!inFile.is_open()) {
        result = "Error opening file.";
        return;
    }

    string line;
    while (getline(inFile, line)) {
        for (char ch : line) {
            result += ch;
            if (ch == ' ') {
                result.append(insertions, '.');
            }
        }
        result += '\n';
    }
    inFile.close();
}

int main() {
    const char IP_SERV[] = "127.0.0.1";
    const int PORT_NUM = 1234;
    const short BUFF_SIZE = 1024;

    int erStat;
    in_addr ip_to_num;
    erStat = inet_pton(AF_INET, IP_SERV, &ip_to_num);

    if (erStat <= 0) {
        cout << "Error in IP translation to special numeric format" << endl;
        return 1;
    }

    WSADATA wsData;
    erStat = WSAStartup(MAKEWORD(2, 2), &wsData);

    if (erStat != 0) {
        cout << "Error WinSock version initialization #" << WSAGetLastError() << endl;
        return 1;
    }
    else {
        cout << "WinSock is initialized" << endl;
    }

    SOCKET ServSock = socket(AF_INET, SOCK_STREAM, 0);

    if (ServSock == INVALID_SOCKET) {
        cout << "Error initialization socket #" << WSAGetLastError() << endl;
        WSACleanup();
        return 1;
    }
    else {
        cout << "Server socket is initialized" << endl;
    }

    sockaddr_in servInfo;
    ZeroMemory(&servInfo, sizeof(servInfo));
    servInfo.sin_family = AF_INET;
    servInfo.sin_addr = ip_to_num;
    servInfo.sin_port = htons(PORT_NUM);

    erStat = bind(ServSock, (sockaddr*)&servInfo, sizeof(servInfo));
    if (erStat != 0) {
        cout << "Error socket binding. Error #" << WSAGetLastError() << endl;
        closesocket(ServSock);
        WSACleanup();
        return 1;
    }
    else {
        cout << "Socket binding completed" << endl;
    }

    erStat = listen(ServSock, SOMAXCONN);
    if (erStat != 0) {
        cout << "Can't start to listen. Error #" << WSAGetLastError() << endl;
        closesocket(ServSock);
        WSACleanup();
        return 1;
    }
    else {
        cout << "Listening" << endl;
    }

    sockaddr_in clientInfo;
    int clientInfo_size = sizeof(clientInfo);
    SOCKET ClientConn = accept(ServSock, (sockaddr*)&clientInfo, &clientInfo_size);

    if (ClientConn == INVALID_SOCKET) {
        cout << "Error accepting connection. Error #" << WSAGetLastError() << endl;
        closesocket(ServSock);
        WSACleanup();
        return 1;
    }
    else {
        cout << "Connection to client established" << endl;
    }

    vector<char> clientBuff(BUFF_SIZE);
    int packet_size = recv(ClientConn, clientBuff.data(), clientBuff.size(), 0);
    if (packet_size == SOCKET_ERROR) {
        cout << "Can't receive message from client. Error #" << WSAGetLastError() << endl;
        closesocket(ClientConn);
        closesocket(ServSock);
        WSACleanup();
        return 1;
    }

    string input(clientBuff.data());
    size_t pos = input.find(' ');
    string fileName = input.substr(0, pos);
    int insertions = stoi(input.substr(pos + 1));

    // Обрабатываем файл и отправляем результат обратно клиенту
    string result;
    processFile(fileName, insertions, result);

    packet_size = send(ClientConn, result.c_str(), result.size(), 0);
    if (packet_size == SOCKET_ERROR) {
        cout << "Can't send message to client. Error #" << WSAGetLastError() << endl;
    }

    closesocket(ClientConn);
    closesocket(ServSock);
    WSACleanup();
    return 0;
}
