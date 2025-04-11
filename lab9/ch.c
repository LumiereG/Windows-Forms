protected override void OnMouseDown(MouseEventArgs e)
{
    base.OnMouseDown(e);

    if (e.Button == MouseButtons.Left && !IsEmpty)
    {
        // Начинаем операцию перетаскивания
        DoDragDrop(this, DragDropEffects.Move);
    }
}

protected override void OnDragEnter(DragEventArgs e)
{
    base.OnDragEnter(e);

    if (e.Data.GetDataPresent(typeof(VialControl)))
    {
        var sourceVial = e.Data.GetData(typeof(VialControl)) as VialControl;

        if (sourceVial != null && sourceVial != this)
            e.Effect = DragDropEffects.Move;
        else
            e.Effect = DragDropEffects.None;
    }
    else
    {
        e.Effect = DragDropEffects.None;
    }
}

protected override void OnDragDrop(DragEventArgs e)
{
    base.OnDragDrop(e);

    if (!e.Data.GetDataPresent(typeof(VialControl)))
        return;

    var sourceVial = e.Data.GetData(typeof(VialControl)) as VialControl;

    if (sourceVial == null || sourceVial == this)
        return;

    var topBlock = sourceVial.GetTopBlockInfo();
    if (topBlock == null)
        return;

    Color pourColor = topBlock.Item1;
    int pourCount = topBlock.Item2;

    bool canPour = false;

    if (IsEmpty)
    {
        canPour = true;
    }
    else
    {
        canPour = PeekTopColor() == pourColor;
    }

    if (canPour && (CurrentSegments + pourCount <= MaxSegments))
    {
        // Переливаем весь блок
        var blockToMove = sourceVial.RemoveTopBlock(pourCount);
        AddSegmentBlock(blockToMove);

        Invalidate();
        sourceVial.Invalidate();
    }
    else
    {
        System.Media.SystemSounds.Beep.Play();
    }
}
