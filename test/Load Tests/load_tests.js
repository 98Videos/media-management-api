import http from 'k6/http';
import { check, sleep } from 'k6';

export let options = {
    stages: [
        { duration: '30s', target: 10 }, 
        { duration: '2m', target: 50 }, 
        { duration: '10s', target: 0 }, 
    ],
};

const binFile = open('./barrel.wmv', 'b');

export default function () {
    const token = 'eyJraWQiOiJpMWk1RjNWSW04bXFWYXN5OVZTdlwvNHk1bjJiY2s4WElPenhiVWkxdGgxVT0iLCJhbGciOiJSUzI1NiJ9.eyJzdWIiOiJlNGM4OTQyOC0yMGQxLTcwZGUtYWM5YS1kZDQ5ZmEzMTUzODQiLCJpc3MiOiJodHRwczpcL1wvY29nbml0by1pZHAudXMtZWFzdC0xLmFtYXpvbmF3cy5jb21cL3VzLWVhc3QtMV8zd2w5M0gwTmoiLCJ2ZXJzaW9uIjoyLCJjbGllbnRfaWQiOiI2ZjdoM3JjOHY0bzYyaTdnMmgyZzM3aHZrYyIsIm9yaWdpbl9qdGkiOiI0MjdjNmI4NS1kYmNkLTQ4ZmEtOGIyZC00YWU3ZGVlNDI3ZGIiLCJldmVudF9pZCI6IjdhOTI2MDcxLTUxYWItNDI0OS1hMTQxLTkwODY0ZWVkZTczNiIsInRva2VuX3VzZSI6ImFjY2VzcyIsInNjb3BlIjoib3BlbmlkIiwiYXV0aF90aW1lIjoxNzM4OTc5NTAyLCJleHAiOjE3Mzg5ODMxMDIsImlhdCI6MTczODk3OTUwMiwianRpIjoiYTM0M2M4YzgtNTc1OS00ZjMxLThiNTUtZDVlMzE2MmVkMjFjIiwidXNlcm5hbWUiOiJhbmRyZSJ9.ineuepa6FX0-6UccUN5lVokw216NlnWOcnQOFJEvCVkoVkjppC_aUm8IRUmY86Jxjj07FYFWKaEXjxA-H29U4GuXt-dHzISY7oVpEYbAiJAUEfyVPYKZkylF-1aWyEDClH3xuo9sQloPnaai-St8FfiqJjkhQRVEmKyD7KTsSemcSgOVwSbOsVpQr65SaYc8zajocntpE6S0OcYxXeubAFOCDxhAdoO9Mcj3yF_1SXVjXCMzo3SL7pIx2hPKa5ULch68pr479jHDWXkI97aSTFzK8mfPHuZCXSfTV71YVOkryAeF_RAb-o-Zj4NCUcfM4jx9CHC0wBe5nhQVPtOcvA'
    let url = 'http://a22ec890ed4d144fcbdc6664bac3639a-921841546.us-east-1.elb.amazonaws.com/api/videos/upload'; 

    const data = {
        file: http.file(binFile, 'barrel.wmv'),
    };


    let res = http.post(url, data, {
        headers: {
            'Authorization': `Bearer ${token}`
        }
    });

    check(res, {
        'is status 200': (r) => r.status === 200, 
    });

    sleep(1);
}