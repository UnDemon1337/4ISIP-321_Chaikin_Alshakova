#include <iostream>
#include <fstream>
#include <string>
#include <cctype>
#include <algorithm> // Добавьте этот заголовок для std::count_if

// Функция для обработки файла
int replaceDigitsInFile(const std::string& filename, char replacementChar) {
    std::ifstream inputFile(filename);
    if (!inputFile) {
        std::cerr << "Ошибка открытия файла: " << filename << "\n";
        return -1;
    }

    std::string content;
    std::string line;

    // Читаем файл построчно
    while (std::getline(inputFile, line)) {
        // Заменяем цифры на заданный символ
        for (char& c : line) {
            if (isdigit(c)) {
                c = replacementChar;
            }
        }
        content += line + '\n'; // Добавляем обработанную строку в содержимое
    }

    inputFile.close(); // Закрываем входной файл

    // Записываем результат обратно в тот же файл
    std::ofstream outputFile(filename);
    if (!outputFile) {
        std::cerr << "Ошибка открытия файла для записи: " << filename << "\n";
        return -1;
    }

    outputFile << content; // Записываем обработанный контент
    outputFile.close(); // Закрываем выходной файл

    // Возвращаем количество заменённых цифр
    return std::count_if(content.begin(), content.end(), ::isdigit);
}

int main(int argc, char* argv[]) {
    // Проверяем количество аргументов командной строки
    if (argc != 3) {
        std::cerr << "Использование: " << argv[0] << " <имя входного файла> <символ замены>\n";
        return -1;
    }

    std::string inputFileName = argv[1];
    char replacementChar = argv[2][0];

    // Вызываем функцию обработки файла
    int replacedCount = replaceDigitsInFile(inputFileName, replacementChar);

    if (replacedCount >= 0) {
        std::cout << "Обработка завершена. Заменено цифр: " << replacedCount << "\n";
    }
    else {
        std::cout << "Произошла ошибка при обработке файла.\n";
    }

    return 0; // Возвращаем 0 в случае успешного выполнения
}
