const alertPlaceholder = document.getElementById('liveAlertPlaceholder')
const visualLoading = document.getElementById('visual-loading');

const alert = (message, type) => {
    const wrapper = document.createElement('div')
    wrapper.innerHTML = [
        `<div class="alert alert-${type} alert-dismissible fade show position-fixed" style="top: 15%; left: 50%; transform: translate(-50%, -50%); z-index: 9999999;" role="alert">`,
        `   <div>${message}</div>`,
        '   <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>',
        '</div>'
    ].join('')

    alertPlaceholder.append(wrapper)

    setTimeout(function () {
        $(".alert").alert('close');
    }, 3000);
}

var electricCost = 0;

function reloadData(data) {
    try {
        // var obj = JSON.parse(data);
        var robots = data;

        if (!robots || robots.length === 0) {
            document.getElementById("robotGrid").innerHTML = `
            <div class="col-12"><p>Không có dữ liệu robot</p></div>
        `;
            return;
        }

        // build HTML động
        let html = "";
        robots.forEach(robot => {
            html += `
            <div class="robot-card" data-bs-target="#robotModal" onclick="loadRobotDetails('${robot.sn}','${robot.name}','${robot.company_id}','${robot.battery}','${robot.is_online}', '${robot.img_name}','${robot.is_online}','${robot.map_name}')">
                <div class="row">
                    <div class="col-3">
                        <img src="/assets/img/${robot.img_name}" alt="Image" class="img-fluid" />
                    </div>
                    <div class="col-9">
                        <div class="row text-start">
                            <div class="col-6 fw-bold">Tên:</div>
                            <div class="col-6">${robot.name}</div>

                            <div class="col-6 fw-bold">Pin:</div>
                            <div class="col-6">${robot.battery} %</div>

                            <div class="col-6 fw-bold">Trạng thái:</div>
                            <div class="col-6">
                                <span >
                                   ${robot.is_online === 1 ? "Online" : "Offline"}
                                </span>
                            </div>
                            <div class="col-6 fw-bold">Trạng thái hoạt động:</div>
                            <div class="col-6">
                                <span class="badge ${robot.work_msg === "空闲" ? "bg-success" : "bg-danger"}">
                                    ${robot.work_msg === "空闲" ? "Sẵn sàng nhận lệnh" : "Đang thực hiện nhiệm vụ"}
                                </span>
                            </div>

                            <div class="col-6 fw-bold">Bản đồ:</div>
                            <div class="col-6">${robot.map_name}</div>

                        </div>
                    </div>
                </div>
            </div>
        `;
        });

        document.getElementById("robotGrid").innerHTML = html;
    }
    catch (ex) {
        console.log(ex);
    }
    finally {
        visualLoading.style.display = 'none';
    }
}



// Biến lưu SN đang xem modal
let currentRobotSN;
let abortController;
let currentTaskIds = []; // lưu danh sách các Taskid
let selectedPoints = []; // Danh sách các điểm đã chọn



function loadRobotDetails(sn, name, company_id, battery, is_online, img_name, is_online, map_name) {
    currentRobotSN = sn;
    abortController = new AbortController();
    //// Show modal
    //document.getElementById('robotDetailContent').innerHTML = "<div class='text-center'></div>";

    const base_img_path = '/assets/img/';
    const img = document.getElementById('robotImage');
    if (img) {
        img.src = base_img_path + img_name;
        // Nếu ảnh không load được
    }
    const rbName = document.getElementById('robotName');
    if (rbName) {
        rbName.textContent = name;
    }
    const modal = new bootstrap.Modal(document.getElementById('robotModal'));
    modal.show();
    // Gọi server để gửi dữ liệu realtime cho robot này
    connectionDetail.invoke("SendRealTimeDataForRobot", sn)
        .catch(err => console.error(err));

    //// Lắng nghe dữ liệu riêng robot này
    connectionDetail.off(`RobotUpdate-${sn}`); // xóa listener cũ nếu có
    connectionDetail.on(`RobotUpdate-${sn}`, (data) => {
        reloadDataDetail(data);
    });
    1
    // Hủy realtime khi đóng modal
    // Khi đóng modal → dừng stream
    // ✅ Khi modal đóng → gọi Stop + gỡ listener
    const modalElement = document.getElementById('robotModal');
    modalElement.addEventListener('hidden.bs.modal', () => {
        connectionDetail.invoke("StopRealTimeData", sn)
            .then(() => console.log(`Stopped realtime for ${sn}`))
            .catch(err => console.error(err));

        // Gỡ listener sau khi đóng modal
        connectionDetail.off(`RobotUpdate-${sn}`);
    }, { once: true });


    try {
        renderPoints(sn, map_name);
    } catch (err) {
        console.error('Lỗi khi load danh sách point:', err);
    }
}

function reloadDataDetail(data) {
    try {
        // var obj = JSON.parse(data);
        var robots = data;


        const robotBattery = document.getElementById('robotBattery');
        const robotIs_online = document.getElementById('robotIs_online');
        const robotMap = document.getElementById('robotMap');
        const robotWorkMes = document.getElementById('robotWorkMes');
        const robotMac = document.getElementById('robotMac');


        if (robotBattery) {
            robotBattery.textContent = data.battery;
        }
        if (robotIs_online) {
            if (data.isOnline == "1") {
                robotIs_online.textContent = "Online";
            }
            else {
                robotIs_online.textContent = "Ofline";
            }
        }
        if (robotIs_online) {
            if (data.workMsg == "空闲") {
                robotWorkMes.classList.remove('bg-success', 'bg-danger');
                robotWorkMes.classList.add('badge', 'bg-success');
                robotWorkMes.textContent = 'Sãn sàng nhận lệnh';
            } else {
                robotWorkMes.classList.remove('bg-success', 'bg-danger');
                robotWorkMes.classList.add('badge', 'bg-danger');
                robotWorkMes.textContent = 'Đang thực hiện nhiệm vụ';
            }
        }
        if (robotMap) {
            robotMap.textContent = data.mapName;
        }
        if (robotMac) {
            robotMac.textContent = data.deviceName;
        }
    }
    catch (ex) {
        console.log(ex);
    }
    finally {
        visualLoading.style.display = 'none';
    }
}

async function loadPoints(sn) {
    const resp = await fetch(`/getListPoint?sn=${encodeURIComponent(sn)}`);
    if (!resp.ok) throw new Error("Không gọi được API");
    return resp.json();
}

async function renderPoints(sn, map_name) {
    try {
        const listPoints = await loadPoints(sn);

        const buttonListLeft = document.getElementById("buttonListLeft");
        const buttonListRight = document.getElementById("buttonListRight");

        buttonListLeft.innerHTML = "";
        buttonListRight.innerHTML = "";
        listPoints.forEach((p, index) => {
            const button = document.createElement("button");
            button.className = "btn btn-primary btn-sm m-1";
            button.innerText = p.name;

            button.onclick = () => {
                if (button.parentElement === buttonListLeft) {
                    // tạo stt duy nhất
                    const stt = Date.now();

                    // clone nút nhưng vẫn giữ nguyên class/màu
                    const cloneBtn = document.createElement("button");
                    cloneBtn.className = button.className; // giữ nguyên màu
                    cloneBtn.innerText = p.name;
                    cloneBtn.dataset.stt = stt;

                    // click clone để xóa
                    cloneBtn.onclick = () => {
                        cloneBtn.remove();
                        selectedPoints = selectedPoints.filter(sp => sp.stt !== stt);
                        console.log('selectedPoints remove', selectedPoints);
                    };

                    buttonListRight.appendChild(cloneBtn);

                    // lưu point kèm stt
                    const pointWithMap = { ...p, map_name: map_name, stt: stt };
                    selectedPoints.push(pointWithMap);
                    console.log('selectedPoints add', selectedPoints);
                }
            };

            buttonListLeft.appendChild(button);
        });
    } catch (err) {
       
        alert("Có lỗi khi gọi API");
    }
}

async function callRobotWithSelected(sn) {
    if (!selectedPoints || selectedPoints.length === 0) {
        return;
    }
    const vt = document.getElementById('vitri');
    if (vt) {
        vt.innerText = `Đang đến: ${selectedPoints[0].name}`;
    };
    const btn = document.getElementById('btnThucHien');
    if (btn) {
        btn.disabled = true;          // 🔒 khóa nút
    }
    for (const p of selectedPoints) {
        await callRobot(sn, p.map_name, p.name, p.type);
        await new Promise(resolve => setTimeout(resolve, 2000));
    }
}


async function callRobot(sn, mapName, pointName, pointType) {
    fetch("/custom_call", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
            sn: sn,
            mapName: mapName,
            pointName: pointName,
            pointType: pointType
        })
    })
        .then(res => res.json())
        .then(serverRes => {
            // serverRes.result là chuỗi JSON
            let parsed = JSON.parse(serverRes.result); // parse lần nữa
            console.log("Parsed:", parsed);
            // Lấy task_id
            currentTaskIds.push(parsed.data.task_id);
        })
        .catch(err => console.error("callRobot error", err));
}


// Hàm render dữ liệu vào modal
function renderRobotDetails(data) {
    const base_img_path = '/assets/img/';
    const img = document.getElementById('robotImage');
    if (img) {
        img.src = base_img_path + data.imgName;
        // Nếu ảnh không load được
        img.onerror = () => img.src = basePath + "default-robot.png";
    }
    const rbName = document.getElementById('robotName');
    if (rbName) {
        rbName.textContent = data.name;
    }
}

function removeClassPerformance(element) {
    var classesToRemove = ['bg-data-80', 'bg-data-100', 'bg-data-zero'];

    classesToRemove.forEach(function (className) {
        element.parentElement.classList.remove(className);
    });
}



var connection = new signalR.HubConnectionBuilder()
    .withUrl("/realTimeHub")
    .withAutomaticReconnect()
    .build();

var connectionDetail = new signalR.HubConnectionBuilder()
    .withUrl("/realTimeDetailHub")
    .withAutomaticReconnect()
    .build();

connection.on("ReceiveRealTimeData", (data) => {
    var obj = JSON.parse(data);
    if (window.location.pathname === "/") {
        reloadData(obj.RobotsStatus);
    }
})

connection.on("RobotCustomCallUpdate", (data) => {
    try {
        const obj = typeof data === "string" ? JSON.parse(data) : data;

        // obj.data là object rồi
        const payload = obj.data || {};

        // Nếu data nằm trong obj.data
        const point = obj.point; // dùng optional chaining để tránh lỗi
        const state = obj.state;

        const vt = document.getElementById('vitri');
        if (vt) {
            if (state === 'CALL_COMPLETE') {
                vt.innerText = `Đã đến: ${point}`;
            } else {
                if (state === 'CALL_SUCCESS') {
                    vt.innerText = `Đang đến điểm: ${point}`;
                }
            }

            // Xóa các button khi tất cả các điểm đã hoàn thành
            if ((state === 'CALL_COMPLETE')) {
                const last = selectedPoints[selectedPoints.length - 1].name; // phần tử cuối
                if (last === point) {
                    selectedPoints = [];
                    const buttonListRight = document.getElementById("buttonListRight");
                    buttonListRight.innerHTML = "";
                    const btn = document.getElementById('btnThucHien');
                    if (btn) {
                        btn.disabled = false;          // 🔒 Mở khóa
                    }
                }
            }
        }

    } catch (err) {
        console.error("Lỗi parse JSON:", err);
    }
})





connection.stop().then(() => {
}).catch((err) => {
    console.error("SignalR connection error: " + err.toString());
});

connection.start().then(() => {
    console.log("Connection established.");
}).catch((err) => {
    console.error("SignalR connection error: " + err.toString());
});

connectionDetail.stop().then(() => {
}).catch((err) => {
    console.error("SignalR connection error: " + err.toString());
});

connectionDetail.start().then(() => {
    console.log("Connection detail established .");
}).catch((err) => {
    console.error("SignalR connection error: " + err.toString());
});


function convertTime(date, culture) {
    var year = date.getFullYear();
    var month = (date.getMonth() + 1).toString().padStart(2, '0');
    var day = date.getDate().toString().padStart(2, '0');
    var hours = date.getHours().toString().padStart(2, '0');
    var minutes = date.getMinutes().toString().padStart(2, '0');
    if (culture !== 'vi-VN') {
        return `${year}/${month}/${day} ${hours}:${minutes}`;

    }
    else {
        return `${day}/${month}/${year} ${hours}:${minutes}`;
    }


}
var sideBarStorage = {
    electric: false,
    pac: false,
    air: false
}

