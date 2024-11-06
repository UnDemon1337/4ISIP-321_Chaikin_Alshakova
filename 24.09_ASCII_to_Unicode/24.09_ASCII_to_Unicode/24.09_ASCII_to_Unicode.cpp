#include <windows.h>
#include <stdio.h>
#include <stdlib.h>
#include <string.h>

BOOL Asc2Un(LPCTSTR fIn, LPCTSTR fOut, BOOL bFailIfExists) {
    HANDLE hIn, hOut;
    DWORD fsLow;
    BOOL result = FALSE;

    // Открыть входной файл с правами на чтение
    hIn = CreateFile(fIn, GENERIC_READ, 0, NULL, OPEN_EXISTING, FILE_ATTRIBUTE_NORMAL, NULL);
    if (hIn == INVALID_HANDLE_VALUE) {
        wprintf(L"Ошибка открытия входного файла: %lu\n", GetLastError());
        return FALSE;
    }

    // Получить размер входного файла
    fsLow = GetFileSize(hIn, NULL);
    if (fsLow == INVALID_FILE_SIZE) {
        wprintf(L"Ошибка получения размера входного файла: %lu\n", GetLastError());
        CloseHandle(hIn);
        return FALSE;
    }

    // Создать выходной файл с правами на запись
    DWORD dwOutCreationDisposition = bFailIfExists ? CREATE_NEW : CREATE_ALWAYS;
    hOut = CreateFile(fOut, GENERIC_WRITE, 0, NULL, dwOutCreationDisposition, FILE_ATTRIBUTE_NORMAL, NULL);
    if (hOut == INVALID_HANDLE_VALUE) {
        wprintf(L"Ошибка открытия выходного файла: %lu\n", GetLastError());
        CloseHandle(hIn);
        return FALSE;
    }

    // Прочитать входной файл
    DWORD bytesRead;
    char* buffer = (char*)calloc(fsLow + 1, sizeof(char)); // Выделение памяти для буфера
    if (!buffer) {
        wprintf(L"Ошибка выделения памяти\n");
        CloseHandle(hIn);
        CloseHandle(hOut);
        return FALSE;
    }

    // Чтение входного файла
    if (!ReadFile(hIn, buffer, fsLow, &bytesRead, NULL)) {
        wprintf(L"Ошибка чтения входного файла: %lu\n", GetLastError());
        free(buffer);
        CloseHandle(hIn);
        CloseHandle(hOut);
        return FALSE;
    }
    buffer[bytesRead] = '\0'; // Завершение строки

    // Вывод содержимого буфера для отладки
    wprintf(L"Содержимое буфера: %S\n", buffer);

    // Преобразовать ASCII-коды в Unicode
    char* context = NULL; // Для strtok_s
    for (char* token = strtok_s(buffer, " ", &context); token != NULL; token = strtok_s(NULL, " ", &context)) {
        int asciiValue = atoi(token); // Преобразовать строку в целое число
        WCHAR unicodeChar = (WCHAR)asciiValue; // Преобразование в WCHAR
        DWORD dwBytesWritten;

        if (!WriteFile(hOut, &unicodeChar, sizeof(WCHAR), &dwBytesWritten, NULL) || dwBytesWritten != sizeof(WCHAR)) {
            wprintf(L"Ошибка записи в выходной файл: %lu\n", GetLastError());
            free(buffer);
            CloseHandle(hIn);
            CloseHandle(hOut);
            return FALSE;
        }
    }

    // Запись завершающего нуля
    WCHAR nullChar = L'\0';
    DWORD dwBytesWritten;
    WriteFile(hOut, &nullChar, sizeof(WCHAR), &dwBytesWritten, NULL);

    // Освободить ресурсы и закрыть дескрипторы
    free(buffer);
    CloseHandle(hIn);
    CloseHandle(hOut);

    return TRUE;
}

int wmain(int argc, wchar_t* argv[]) {
    if (argc < 3) {
        wprintf(L"Использование: %s <входной файл> <выходной файл>\n", argv[0]);
        return 1;
    }

    LPCTSTR inputFile = argv[1];
    LPCTSTR outputFile = argv[2];
    BOOL bFailIfExists = TRUE;

    if (Asc2Un(inputFile, outputFile, bFailIfExists)) {
        wprintf(L"Файл успешно преобразован из ASCII в Unicode.\n");
    }
    else {
        wprintf(L"Ошибка преобразования файла.\n");
    }

    return 0;
}
