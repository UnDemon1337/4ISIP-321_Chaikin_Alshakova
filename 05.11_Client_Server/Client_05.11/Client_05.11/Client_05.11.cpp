#include <iostream>
#include <WinSock2.h>
#include <WS2tcpip.h>
#include <vector>
#include <string>
#include <fstream>

#pragma comment(lib, "Ws2_32.lib")

using namespace std;

int main(int argc, char* argv[]) {
    if (argc < 3) {
        cout << "Usage: " << argv[0] << " <input_file_name> <insertion_count>" << endl;
        return 1;
    }

    const char SERVER_IP[] = "127.0.0.1";
    const short SERVER_PORT_NUM = 1234;
    const short BUFF_SIZE = 1024;

    int erStat;
    in_addr ip_to_num;
    erStat = inet_pton(AF_INET, SERVER_IP, &ip_to_num);

    WSADATA wsData;
    erStat = WSAStartup(MAKEWORD(2, 2), &wsData);

    if (erStat != 0) {
        cout << "Error WinSock version initialization #" << WSAGetLastError() << endl;
        return 1;
    }
    else {
        cout << "WinSock is initialized" << endl;
    }

    SOCKET ClientSock = socket(AF_INET, SOCK_STREAM, 0);
    if (ClientSock == INVALID_SOCKET) {
        cout << "Error initialization socket #" << WSAGetLastError() << endl;
        WSACleanup();
        return 1;
    }
    else {
        cout << "Client socket is initialized" << endl;
    }

    sockaddr_in servInfo;
    ZeroMemory(&servInfo, sizeof(servInfo));
    servInfo.sin_family = AF_INET;
    servInfo.sin_addr = ip_to_num;
    servInfo.sin_port = htons(SERVER_PORT_NUM);

    erStat = connect(ClientSock, (sockaddr*)&servInfo, sizeof(servInfo));

    if (erStat != 0) {
        cout << "Connection to Server failed. Error #" << WSAGetLastError() << endl;
        closesocket(ClientSock);
        WSACleanup();
        return 1;
    }
    else {
        cout << "Connection is established. Ready to send message to Server" << endl;
    }

    // Передаем имя файла и количество вставок серверу
    string fileName = argv[1];
    string insertionCount = argv[2];
    string message = fileName + " " + insertionCount;

    int packet_size = send(ClientSock, message.c_str(), message.size(), 0);
    if (packet_size == SOCKET_ERROR) {
        cout << "Can't send message to Server. Error #" << WSAGetLastError() << endl;
        closesocket(ClientSock);
        WSACleanup();
        return 1;
    }

    // Получаем обработанные данные от сервера
    vector<char> servBuff(BUFF_SIZE);
    packet_size = recv(ClientSock, servBuff.data(), servBuff.size(), 0);

    if (packet_size == SOCKET_ERROR) {
        cout << "Can't receive message from Server. Error #" << WSAGetLastError() << endl;
        closesocket(ClientSock);
        WSACleanup();
        return 1;
    }
    else {
        cout << "Processed file content from server: " << servBuff.data() << endl;
    }

    closesocket(ClientSock);
    WSACleanup();

    return 0;
}
