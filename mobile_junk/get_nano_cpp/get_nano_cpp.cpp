//
//
//

#define _CRT_SECURE_NO_WARNINGS

#include <string>
#include <WinSock2.h>
#include <Ws2tcpip.h>

#include <stdint.h>

#include <cassert>

#pragma comment(lib, "ws2_32.lib")
using namespace std;

int main() 
{
    const char* pkt = "CONNECT\r\n";
    const char* srcIP = "0.0.0.0";
    const char* destIP = "192.168.1.136";
    int destPort = 2390;
    sockaddr_in dest;
    sockaddr_in local;
    WSAData data;

    int ok = WSAStartup(MAKEWORD(2, 2), &data);
    assert(ok == 0);

    local.sin_family = AF_INET;
    inet_pton(AF_INET, srcIP, &local.sin_addr.s_addr);
    local.sin_port = htons(0);

    dest.sin_family = AF_INET;
    inet_pton(AF_INET, destIP, &dest.sin_addr.s_addr);
    dest.sin_port = htons(destPort);

    SOCKET s = socket(AF_INET, SOCK_DGRAM, IPPROTO_UDP);
    bind(s, (sockaddr*)&local, sizeof(local));

    ok = sendto(s, pkt, (int)strlen(pkt), 0, (sockaddr*)&dest, (int)sizeof(dest));
    assert(ok == strlen(pkt));

    fd_set fdset;
    FD_ZERO(&fdset);
    FD_SET(s, &fdset);
    timeval tmo = { 10, 0 }; // ten seconds

    ok = select(1, &fdset, NULL, NULL, &tmo);
    assert(ok != -1);

    if (ok == 1)
    {
        char buffer[1024];
        ok = recv(s, buffer, sizeof(buffer), 0);
        assert(ok > 0);

        printf("Recv: %d\n", ok);
        fwrite(buffer, ok, 1, stdout);

        FD_ZERO(&fdset);
        FD_SET(s, &fdset);
        ok = select(1, &fdset, NULL, NULL, &tmo);
        if (ok == 1) {
            struct rec_t {
                uint32_t stamp;
                int32_t x, y, z;
                uint32_t h, c, f;
            } rec{};
            ok = recv(s, (char *)&rec, sizeof(rec), 0);
            if (ok > 0)
            {
                printf("Recv: %d\n", ok);
                printf("Data: %d, %d, %d, %d\n", rec.stamp, rec.h, rec.c, rec.f);
            }
            else {
                printf("Error: %d\n", WSAGetLastError());
            }
        }
        else {
            printf("Timeout on reading temperature...\n");
        }

    }

    const char *pkt2 = "CLOSE\r\n";
    ok = sendto(s, pkt2, (int)strlen(pkt2), 0, (sockaddr*)&dest, (int)sizeof(dest));
    closesocket(s);
    WSACleanup();

    return 0;
}