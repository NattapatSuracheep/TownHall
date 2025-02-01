public abstract class AtlantisComponent
{
    protected AtlantisAtlasPacker atlantis;

    public virtual void Initialize(AtlantisAtlasPacker atlantis)
    {
        this.atlantis = atlantis;
    }
}