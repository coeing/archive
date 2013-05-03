import QtQuick 1.0

Rectangle {
    id: page
    width: 360
    height: 360
    color: "#343434"
    MouseArea {
        anchors.fill: parent
        onClicked: {
            Qt.quit();
        }

        Image {
            id: image1
            x: 260
            y: 0
            width: 100
            height: 100
            source: "../../circli/trunk/images/smiley.png"
        }
    }
}
