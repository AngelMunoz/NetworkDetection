namespace NetworkDetection

module Counter =
    open System
    open System.Net.NetworkInformation
    open Avalonia.Controls
    open Avalonia.FuncUI.DSL
    open Avalonia.Layout
    open Avalonia.Controls.Notifications

    type State =
        { count: int
          isNetworkUp: bool
          notificationManager: WindowNotificationManager }

    let init notificationManager =
        let isDown =
            NetworkInterface.GetAllNetworkInterfaces()
            |> Seq.where (fun network ->
                network.OperationalStatus
                |> int = 1)
            |> Seq.isEmpty
        { isNetworkUp = not isDown
          count = 0
          notificationManager = notificationManager }

    type Msg =
        | Increment
        | Decrement
        | Reset
        | NetworkChanged of bool

    let update (msg: Msg) (state: State): State =
        match msg with
        | Increment -> { state with count = state.count + 1 }
        | Decrement -> { state with count = state.count - 1 }
        | Reset -> init state.notificationManager
        | NetworkChanged isUp ->
            let message = sprintf "Network status is %s" (if isUp then "Up" else "Down")
            let time = Nullable(TimeSpan.FromSeconds 3.5)
            let notification = Notification("Network Changed", message, NotificationType.Information, time)
            state.notificationManager.Show(notification)
            { state with isNetworkUp = isUp }

    let view (state: State) (dispatch) =
        DockPanel.create
            [ DockPanel.children
                [ Button.create
                    [ Button.dock Dock.Bottom
                      Button.onClick (fun _ -> dispatch Reset)
                      Button.content "reset" ]
                  Button.create
                      [ Button.dock Dock.Bottom
                        Button.onClick (fun _ -> dispatch Decrement)
                        Button.content "-" ]
                  Button.create
                      [ Button.dock Dock.Bottom
                        Button.onClick (fun _ -> dispatch Increment)
                        Button.content "+" ]
                  TextBlock.create
                      [ TextBlock.dock Dock.Top
                        TextBlock.fontSize 32.0
                        TextBlock.verticalAlignment VerticalAlignment.Center
                        TextBlock.horizontalAlignment HorizontalAlignment.Left
                        TextBlock.text (sprintf "Network is %s" (if state.isNetworkUp then "Up" else "Down")) ]
                  TextBlock.create
                      [ TextBlock.dock Dock.Top
                        TextBlock.fontSize 48.0
                        TextBlock.verticalAlignment VerticalAlignment.Center
                        TextBlock.horizontalAlignment HorizontalAlignment.Center
                        TextBlock.text (string state.count) ] ] ]
